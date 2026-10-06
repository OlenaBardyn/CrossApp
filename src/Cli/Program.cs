using Core;
using Core.Abstractions;
using Core.Domain;
using Core.Services;
using Core.Storage;

Console.OutputEncoding = System.Text.Encoding.UTF8;

bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

IBookStore store = useFile
    ? new FileBookStore(dataPath)
    : new InMemoryBookStore(SampleData.Books());

var service = new LendingService(store);

Console.WriteLine($"Сховище: {store.GetType().Name}");
Console.WriteLine(new string('-', 60));

var created = service.AddBook("978-966-99-0001-1", "Нова книга", 2026);
Console.WriteLine($"Створено: {created}");

service.IssueCopy(created.Id);
Console.WriteLine($"Видано: {service.Find(created.Id)}");

service.ReturnCopy(created.Id);
Console.WriteLine($"Повернено: {service.Find(created.Id)}");

Console.WriteLine();
Console.WriteLine("Усі записи:");
foreach (var p in service.All())
    Console.WriteLine($" {p.Id,-10} {p.Isbn,-22} {p.Title,-32} {p.Year,-10} {(p.IsIssued ? "видано" : "в наявності")}");

Console.WriteLine();
Console.WriteLine("Сценарій відмови");

TryDo("дубль id", () =>
{
    var duplicate = BookCopy.Create(created.Id, "978-966-99-0002-2", "Дубль", 2026);
    store.Add(duplicate);
});

TryDo("видача неіснуючого id", () => service.IssueCopy("nonexistent"));

static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($" {title}: OK");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" {title}: {ex.GetType().Name} — {ex.Message}");
    }
}