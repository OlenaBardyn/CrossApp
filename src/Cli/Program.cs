using System.Reflection;
using Core;
using Core.Abstractions;
using Core.Configuration;
using Core.Domain;
using Core.Events;
using Core.Reports;
using Core.Services;
using Core.Storage;
using Microsoft.Extensions.Configuration;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// налаштування конфігуряції
var envName = Environment.GetEnvironmentVariable("CROSSAPP_ENVIRONMENT") ?? "Development";
var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{envName}.json", optional: true)
    .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
    .AddEnvironmentVariables(prefix: "CROSSAPP_")
    .Build();

var options = config.Get<AppOptions>() ?? new AppOptions();

var legacy = Environment.GetEnvironmentVariable("CROSSAPP_DATA_PATH");
var storageOptions = new StorageOptions
{
    DataPath = string.IsNullOrWhiteSpace(legacy) ? options.Storage.DataPath : legacy,
    FileName = options.Storage.FileName
};

string dataPath = storageOptions.ResolveFullPath(AppContext.BaseDirectory);

Console.WriteLine($"Файл даних : {dataPath}");
var apiKey = config["Integration:ApiKey"];
Console.WriteLine(apiKey is null ? "ApiKey: не заданий" : $"ApiKey: присутній, довжина {apiKey.Length}");
Console.WriteLine(new string('-', 60));


using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

IBookStore store = StoreFactory.Create(args, dataPath);
var service = new LendingService(store);

/*var source = service.All().ToList();
var query = source.Where(b => b.Year < 1900);
Console.WriteLine($"Старих книг до змін: {query.Count()}");

source.Add(BookCopy.Create("TMP-001", "978-000-00-0000-0", "Стара книга", 1850));
Console.WriteLine($"Старих книг після змін: {query.Count()}");

var snapshot = query.ToList();
source.Clear();
Console.WriteLine($"У знімку лишилось: {snapshot.Count}");
Console.WriteLine(new string('-', 60));  */


var items = service.All();
var command = args.FirstOrDefault(a => !a.StartsWith("--")) ?? "report-top";

switch (command)
{
    case "report-top":
        Print(ReportService.NewestBooks(items, 5));
        break;
    case "report-old":
        Print(ReportService.OldBooks(items, 1900));
        break;
    case "report-groups":
        foreach (var g in ReportService.ByYear(items))
            Console.WriteLine($"Книг {g.Key} року: {g.Count,3}");
        break;
    case "report-sum":
        var t = ReportService.Totals(items);
        Console.WriteLine($" всього {t.Items}, видано {t.Issued}, середній рік {t.AvgYear:F0}, найновіший {t.MaxYear}");
        break;
    case "report-join":
        Console.WriteLine("Книги з видачами:");
        foreach (var l in ReportService.JoinBooksWithLoans(items))
            Console.WriteLine($" {l.Isbn,-22} {l.Title,-30} -> {l.ReaderId}");
        break;
    case "report-longest":
        Print(ReportService.TopLongestTitles(items, 5));
        break;

    case "report-import":
        {
            Console.WriteLine();
            Console.WriteLine("Асинхронний імпорт");
            try
            {
                Console.WriteLine("Імпорт почався. Натисніть Ctrl+C для скасування...");
                var added = await service.ImportAsync(new JsonBookImporter(), dataPath, cts.Token);
                Console.WriteLine($"Імпортовано записів: {added}");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Імпорт скасовано: користувач натиснув Ctrl+C");
            }
            catch (DomainException ex) { Console.WriteLine($"Дані відхилено: {ex.Message}"); }
            catch (FileNotFoundException) { Console.WriteLine($"Файл не знайдено: {dataPath}"); }
            break;
        }

    case "report-progress":
        {
            Console.WriteLine();
            Console.WriteLine("Імпорт з прогрес-баром");

            var progress = new Progress<int>(percent =>
            {
                Console.Write($"\rІмпорт: {percent}% ");
            });

            try
            {
                var added = await service.ImportAsync(new JsonBookImporter(), dataPath, cts.Token, progress);
                Console.WriteLine();
                Console.WriteLine($"Імпортовано записів: {added}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nПомилка: {ex.Message}");
            }
            break;
        }

    case "report-stream":
        Console.WriteLine("Потоковий імпорт:");
        var importer = new JsonBookImporter();
        try
        {
            await foreach (var book in importer.LoadStreamAsync(dataPath, cts.Token))
            {
                Console.WriteLine($"Отримано: {book.Title} ({book.Year})");
            }
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
        break;


    case "export":
        var books = service.All();

        if (books.Count == 0)
        {
            books = SampleData.Books().ToList();
            if (store is FileBookStore fileStore)
            {
                foreach (var b in books)
                {
                    try { fileStore.Add(b); } catch { } 
                }
            }
        }

        Console.WriteLine($"Експортовано {books.Count} записів.");

        if (File.Exists(dataPath))
        {
            Console.WriteLine("Вміст файлу:");
            Console.WriteLine(File.ReadAllText(dataPath));
        }
        break;

    default:
        Console.WriteLine("Команди: report-top | report-old | report-groups | report-sum | report-import | export");
        break;
}

static void Print(IReadOnlyList<ReportLine> lines)
{
    foreach (var l in lines)
        Console.WriteLine($" {l.Isbn,-22} {l.Title,-32} {l.Year,5} {(l.IsIssued ? "видано" : "в наявності")}");
}