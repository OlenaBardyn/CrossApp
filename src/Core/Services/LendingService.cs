using Core.Abstractions;
using Core.Domain;
using Core.Events;                 

namespace Core.Services;

public sealed class LendingService(IBookStore store)
{
    private readonly IBookStore _store = store ?? throw new ArgumentNullException(nameof(store));

    public event EventHandler<BookChangedEventArgs>? Changed;   

    private void OnChanged(BookCopy copy, BookChangeKind kind, string? readerId = null) =>   
        Changed?.Invoke(this, new BookChangedEventArgs(copy.Id, kind, readerId, copy.IsIssued));  

    public BookCopy AddBook(string isbn, string title, int year)
    {
        var copy = BookCopy.Create(Guid.NewGuid().ToString("N")[..8], isbn, title, year);
        _store.Add(copy);
        OnChanged(copy, BookChangeKind.Added);  
        return copy;
    }

    public void IssueCopy(string id, string readerId)  
    {
        var copy = _store.GetById(id)
            ?? throw new InvalidOperationException($"Немає запису з id={id}.");
        copy.Issue();
        _store.Update(copy);
        OnChanged(copy, BookChangeKind.Issued, readerId);   
    }

    public void ReturnCopy(string id)
    {
        var copy = _store.GetById(id)
            ?? throw new InvalidOperationException($"Немає запису з id={id}.");
        copy.Return();
        _store.Update(copy);
        OnChanged(copy, BookChangeKind.Returned);   
    }

    public IReadOnlyList<BookCopy> FindAll(Func<BookCopy, bool> predicate) =>  
        _store.List().Where(predicate).ToList();   

    public IReadOnlyList<BookCopy> All() => _store.List();

    public BookCopy? Find(string id) => _store.GetById(id);
}