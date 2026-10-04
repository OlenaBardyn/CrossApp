using Core.Domain;

using Core.Dto;
using Core.Import;


Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== Сценарій 1: успіх ===");

BookCopy copy = BookCopy.Create("C-001", "978-966-01-0001-1", "Кобзар");
Console.WriteLine(copy);

Loan loan = Loan.Open("L-001", copy, "R-001", new DateTime(2026, 9, 22));
Console.WriteLine(loan);
Console.WriteLine(copy);

loan.Close(new DateTime(2026, 10, 6));
Console.WriteLine(loan);
Console.WriteLine(copy);

Console.WriteLine();
Console.WriteLine("=== Сценарій 2: порушення інваріантів ===");

BookCopy issued = BookCopy.Create("C-099", "978-966-99-0001-1", "Тест");
issued.Issue();
TryDo("повторна видача", () => issued.Issue());
TryDo("порожній ISBN", () => BookCopy.Create("C-002", " ", "Щось"));
TryDo("закриття закритої видачі", () => loan.Close(new DateTime(2026, 10, 10)));
TryDo("повернення раніше видачі",
    () =>
    {
        BookCopy c = BookCopy.Create("C-003", "978-966-01-0003-3", "Місто");
        Loan l = Loan.Open("L-002", c, "R-002", new DateTime(2026, 9, 22));
        l.Close(new DateTime(2026, 9, 1));
    });


//додаткове 1
Console.WriteLine();
Console.WriteLine("Дані + помилки");

ImportResult<BookDto> imported = BookCsvImporter.Load("data/sample.csv");
Console.WriteLine($"Імпортовано DTO: {imported.Items.Count}, помилок: {imported.Errors.Count}");

ImportResult<BookCopy> converted = DtoToEntity.Convert(imported);
Console.WriteLine($"Створено сутностей: {converted.Items.Count}, помилок: {converted.Errors.Count}");

foreach (BookCopy bc in converted.Items.Take(5))
    Console.WriteLine($" {bc}");

if (converted.Errors.Count > 0)
{
    Console.WriteLine();
    Console.WriteLine("Помилки:");
    foreach (string e in converted.Errors)
        Console.WriteLine($" ! {e}");
}
// 

static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($" {title}: виняток НЕ спрацював — інваріант відсутній!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" {title}: {ex.GetType().Name} — {ex.Message}");
    }
}