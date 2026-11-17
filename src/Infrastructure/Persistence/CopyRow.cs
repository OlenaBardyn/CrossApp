namespace Infrastructure.Persistence;

public sealed class CopyRow
{
    public int Id { get; set; } // Первинний ключ 
    public string BookId { get; set; } = ""; // Зовнішній ключ
    public bool IsIssued { get; set; }

    public BookRow Book { get; set; } = null!;
}