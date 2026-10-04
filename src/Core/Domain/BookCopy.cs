using Core.Dto;

namespace Core.Domain;

public sealed class BookCopy
{
    private BookCopy(string id, string isbn, string title)
    {
        Id = id;
        Isbn = isbn;
        Title = title;
        _isIssued = false;
    }

    public string Id { get; }
    public string Isbn { get; }
    public string Title { get; }
    private bool _isIssued;
    public bool IsIssued => _isIssued;

    // фабричний метод
    public static BookCopy Create(string id, string isbn, string title)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Ідентифікатор обов'язковий", nameof(id));

        if (string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("ISBN не може бути порожнім", nameof(isbn));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Назва не може бути порожньою", nameof(title));

        return new BookCopy(id.Trim(), isbn.Trim(), title.Trim());
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
    public BookDto ToDto() => new(Id, Isbn, Title, 0);

    public static BookCopy FromDto(BookDto dto) =>
        Create(dto.Id, dto.Isbn, dto.Title);

    public override string ToString() =>
        $"{Id} [{Isbn}] {Title} — {(IsIssued ? "видано" : "в наявності")}";
}