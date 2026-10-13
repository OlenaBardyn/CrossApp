using Core;
using Core.Abstractions;
using Core.Domain;
using Core.Dto;
using Core.Events;
using Core.Import;
using Core.Services;
using Core.Storage;

Console.OutputEncoding = System.Text.Encoding.UTF8;

IBookStore store = StoreFactory.Create(args);
var service = new LendingService(store);

Console.WriteLine($"Сховище: {store.GetType().Name}");
Console.WriteLine(new string('-', 60));

var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "app.log");
Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

// 1
void OnBookChanged(object? sender, BookChangedEventArgs e)
{
    var line = $"{e.At:HH:mm:ss} {e.Kind,-8} {e.Id} — {(e.IsIssuedAfter ? "видано" : "в наявності")}";
    Console.WriteLine(line);
    File.AppendAllText(logPath, line + Environment.NewLine);
}

service.Changed += OnBookChanged;

// 2
int changes = 0;
service.Changed += (_, _) => changes++;


var created = service.AddBook("978-966-99-0001-1", "Нова книга", 2026);

service.IssueCopy(created.Id, "R-001");

service.ReturnCopy(created.Id);

Console.WriteLine();
Console.WriteLine($"Подій отримано: {changes}");

Console.WriteLine();
Console.WriteLine("Відписка від OnBookChanged");
service.Changed -= OnBookChanged;

service.IssueCopy(created.Id, "R-002");
Console.WriteLine($"Подій отримано: {changes}");

Console.WriteLine();
Console.WriteLine("Сценарій відмови:");

try
{
    service.IssueCopy("nonexistent", "R-003");
}
catch (Exception ex)
{
    Console.WriteLine($" помилка: {ex.GetType().Name} — {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("Делегат. Книги в наявності");
foreach (var p in service.FindAll(b => !b.IsIssued))  //b => b.Year < 1900)
    Console.WriteLine($" {p.Title} ({p.Year})");


// додаткове 2: Action<string> для помилок імпорту
Console.WriteLine();
Console.WriteLine("Пропущені рядки:");

ImportResult<BookDto> imported = BookCsvImporter.Load("data/sample.csv", error => Console.WriteLine($" ! {error}"));

Console.WriteLine($"Імпортовано: {imported.Items.Count}, помилок: {imported.Errors.Count}");