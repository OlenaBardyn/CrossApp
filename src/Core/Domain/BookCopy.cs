using Core.Dto;

namespace Core.Domain;

public sealed class BookCopy
{
    private BookCopy(string id, string isbn, string title, int year)
    {
        Id = id;
        Isbn = isbn;
        Title = title;
        Year = year;
        _isIssued = false;
    }

    public string Id { get; }
    public string Isbn { get; }
    public string Title { get; }
    public int Year { get; }
    private bool _isIssued;
    public bool IsIssued => _isIssued;

    // фабричний метод
    public static BookCopy Create(string id, string isbn, string title, int year)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Ідентифікатор обов'язковий", nameof(id));

        if (string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("ISBN не може бути порожнім", nameof(isbn));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Назва не може бути порожньою", nameof(title));

        if (year < 1450 || year > DateTime.Now.Year)
            throw new ArgumentOutOfRangeException(nameof(year), year,
                "Рік поза допустимими межами");

        return new BookCopy(id.Trim(), isbn.Trim(), title.Trim(), year);
    }

    // методи зміни стану
    public void Issue()
    {
        if (IsIssued)
            throw new InvalidOperationException(
                $"Примірник {Id} вже виданий, повторна видача неможлива");

        _isIssued = true;
    }

    public void Return()
    {
        if (!IsIssued)
            throw new InvalidOperationException(
                $"Примірник {Id} не був виданий, повернення неможливе");

        _isIssued = false;
    }

    // мапінг
    public BookDto ToDto() => new(Id, Isbn, Title, Year);

    public static BookCopy FromDto(BookDto dto) =>
        Create(dto.Id, dto.Isbn, dto.Title, dto.Year);

    public override string ToString() =>
        $"{Id} [{Isbn}] {Title} — {(IsIssued ? "видано" : "в наявності")}";
}