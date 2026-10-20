using Core;
using Core.Abstractions;
using Core.Domain;
using Core.Events;
using Core.Reports;
using Core.Services;
using Core.Storage;

Console.OutputEncoding = System.Text.Encoding.UTF8;

IBookStore store = StoreFactory.Create(args);
var service = new LendingService(store);

var source = service.All().ToList();
var query = source.Where(b => b.Year < 1900);
Console.WriteLine($"Старих книг до змін: {query.Count()}");

source.Add(BookCopy.Create("TMP-001", "978-000-00-0000-0", "Стара книга", 1850));
Console.WriteLine($"Старих книг після змін: {query.Count()}");

var snapshot = query.ToList();
source.Clear();
Console.WriteLine($"У знімку лишилось: {snapshot.Count}");
Console.WriteLine(new string('-', 60));

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

        

    //додаткове 1
    case "report-join":
        Console.WriteLine("Книги з видачами:");
        foreach (var l in ReportService.JoinBooksWithLoans(items))
            Console.WriteLine($" {l.Isbn,-22} {l.Title,-30} -> {l.ReaderId}");
        break;
    //2
    case "report-longest":
        Print(ReportService.TopLongestTitles(items, 5));
        break;


    default:
        Console.WriteLine("report-top | report-old | report-groups | report-sum");
        break;
}

static void Print(IReadOnlyList<ReportLine> lines)
{
    foreach (var l in lines)
        Console.WriteLine($" {l.Isbn,-22} {l.Title,-32} {l.Year,5} {(l.IsIssued ? "видано" : "в наявності")}");
}


// 1
Console.WriteLine();
Console.WriteLine("Stopwatch на 100 000 записів");

var bigData = Enumerable.Range(1, 100_000)
    .Select(i => BookCopy.Create($"ID-{i}", $"ISBN-{i}", $"Книга {i}", 1900 + (i % 100)))
    .ToList();

var sw = System.Diagnostics.Stopwatch.StartNew();


var queryWithoutToList = bigData.Where(b => b.Year > 1950).OrderBy(b => b.Year);
sw.Stop();
Console.WriteLine($"Побудова запиту (без ToList): {sw.ElapsedMilliseconds} мс");

sw.Restart();

var queryWithToList = bigData.Where(b => b.Year > 1950).OrderBy(b => b.Year).ToList();
sw.Stop();
Console.WriteLine($"Виконання і матеріалізація (з ToList): {sw.ElapsedMilliseconds} мс");