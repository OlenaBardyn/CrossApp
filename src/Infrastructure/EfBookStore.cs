using Core.Abstractions;
using Core.Domain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public sealed class EfBookStore(AppDbContext db) : IBookStore
{
    private static BookCopy ToDomain(CopyRow row) =>
        BookCopy.Create(row.BookId, row.Book.Isbn, row.Book.Title, row.Book.Year);

    public IReadOnlyList<BookCopy> List() =>
        db.Copies.AsNoTracking().Include(c => c.Book).AsEnumerable().Select(ToDomain).ToList();

    public BookCopy? GetById(string id) =>
        db.Copies.AsNoTracking().Include(c => c.Book)
          .SingleOrDefault(c => c.BookId == id) is { } row
            ? ToDomain(row) : null;

    public void Add(BookCopy item)
    {
        // Знаходимо або створюємо книгу
        var book = db.Books.SingleOrDefault(b => b.Isbn == item.Isbn);
        if (book is null)
        {
            book = new BookRow
            {
                Id = item.Id, // Зберігаємо ID, який нам прислав API
                Isbn = item.Isbn,
                Title = item.Title,
                Year = item.Year
            };
            db.Books.Add(book);
        }

        // Створюємо примірник і кладемо туди той самий ID
        db.Copies.Add(new CopyRow
        {
            BookId = item.Id,
            IsIssued = item.IsIssued
        });

        db.SaveChanges();
    }

    public void Update(BookCopy item)
    {
        var row = db.Copies.Single(c => c.BookId == item.Id);
        row.IsIssued = item.IsIssued;
        db.SaveChanges();
    }

    public bool Remove(string id)
    {
        var row = db.Copies.SingleOrDefault(c => c.BookId == id);
        if (row is null) return false;
        db.Copies.Remove(row);
        db.SaveChanges();
        return true;
    }
}