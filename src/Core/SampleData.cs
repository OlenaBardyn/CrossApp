using Core.Domain;

namespace Core;

public static class SampleData
{
    public static IEnumerable<BookCopy> Books()
    {
        return new List<BookCopy>
        {
            BookCopy.Create("B-001", "978-966-01-0001-1", "Кобзар", 1840),
            BookCopy.Create("B-002", "978-966-01-0002-2", "Мина Мазайло", 1929),
            BookCopy.Create("B-003", "978-966-01-0003-3", "Тіні забутих предків", 1911),
            BookCopy.Create("B-004", "978-966-01-0004-4", "Лісова пісня", 1911),
            BookCopy.Create("B-005", "978-966-01-0005-5", "Місто", 1928),
            BookCopy.Create("B-006", "978-966-01-0006-6", "Собор", 1968),
            BookCopy.Create("B-007", "978-966-01-0007-7", "Маруся Чурай", 1979),
            BookCopy.Create("B-008", "978-966-01-0008-8", "Тигролови", 1944),
            BookCopy.Create("B-009", "978-966-01-0009-9", "Вершники", 1935),
            BookCopy.Create("B-010", "978-966-01-0010-0", "Захар Беркут", 1883),
            BookCopy.Create("B-011", "978-966-01-0011-1", "Камінний хрест", 1900),
            BookCopy.Create("B-012", "978-966-01-0012-2", "Intermezzo", 1908),
            BookCopy.Create("B-013", "978-966-01-0013-3", "Мойсей", 1905),
            BookCopy.Create("B-014", "978-966-01-0014-4", "Contra spem spero", 1890),
            BookCopy.Create("B-015", "978-966-01-0015-5", "Лісова пісня", 1911)
        };
    }
}