using Core.Domain;

namespace Core.Tests;

public static class TestData
{
    public static IEnumerable<BookCopy> Books() => new List<BookCopy>
    {
        BookCopy.Create("B-1", "978-1", "Книга 1", 1840),
        BookCopy.Create("B-2", "978-2", "Книга 2", 1911),
        BookCopy.Create("B-3", "978-3", "Книга 3", 1929)
    };
}