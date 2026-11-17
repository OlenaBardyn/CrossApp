namespace Infrastructure.Persistence;

public sealed class BookRow
{
    public string Id { get; set; } = "";
    public string Isbn { get; set; } = "";
    public string Title { get; set; } = "";
    public int Year { get; set; }

    // Зв'язок 1:N (одна книга має багато примірників)
    public List<CopyRow> Copies { get; set; } = [];
}