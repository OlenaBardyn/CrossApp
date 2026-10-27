using Core.Domain;

namespace Core.Reports;

public static class ReportService
{
    public static IReadOnlyList<ReportLine> OldBooks(IEnumerable<BookCopy> source, int yearThreshold) =>
        source.Where(b => b.Year < yearThreshold)
              .OrderBy(b => b.Year).ThenBy(b => b.Isbn, StringComparer.Ordinal)
              .Select(b => new ReportLine(b.Isbn, b.Title, b.Year, b.IsIssued))
              .ToList();

    public static IReadOnlyList<ReportLine> NewestBooks(IEnumerable<BookCopy> source, int n) =>
        source.OrderByDescending(b => b.Year).ThenBy(b => b.Isbn, StringComparer.Ordinal)
              .Take(n)
              .Select(b => new ReportLine(b.Isbn, b.Title, b.Year, b.IsIssued))
              .ToList();

    /* //2
    public static IReadOnlyList<ReportLine> NewestBooks(IEnumerable<BookCopy> source, int n) =>
        source.TopBy(b => b.Year, n) 
              .Select(b => new ReportLine(b.Isbn, b.Title, b.Year, b.IsIssued))
              .ToList(); */

    public static IReadOnlyList<GroupSummary> ByYear(IEnumerable<BookCopy> source) =>
        source.GroupBy(b => b.Year)
              .Select(g => new GroupSummary(g.Key.ToString(), g.Count()))
              .OrderByDescending(s => s.Count)
              .ToList();

    //додаткове 3
   /* public static IReadOnlyList<GroupSummary> ByYear(IEnumerable<BookCopy> source) =>
        (from b in source
         group b by b.Year into g
         orderby g.Count() descending
         select new GroupSummary(g.Key.ToString(), g.Count()))
        .ToList(); */


    public static (int Items, int Issued, double AvgYear, int MaxYear) Totals(IEnumerable<BookCopy> source)
    {
        var list = source.ToList();
        return list.Count == 0
            ? (0, 0, 0d, 0)
            : (list.Count, list.Count(b => b.IsIssued),
               list.Average(b => b.Year), list.Max(b => b.Year));                                                                        //InvalidOperationException ("Sequence contains no elements")

    }




    //додаткове 1
    public sealed record NewLoan(string Isbn, string ReaderId);

    public static IReadOnlyList<LoanReport> JoinBooksWithLoans(IEnumerable<BookCopy> source)
    {
        var loans = new List<NewLoan>
    {
        new("978-966-01-0001-1", "Читач1"),
        new("978-966-01-0002-2", "Читач2")
    };

        return source.Join(
            loans,
            book => book.Isbn, 
            loan => loan.Isbn,     
            (book, loan) => new LoanReport(book.Isbn, book.Title, loan.ReaderId))
            .ToList();
    }




    //2
    public static IReadOnlyList<ReportLine> TopLongestTitles(IEnumerable<BookCopy> source, int n) =>
    source.TopBy(b => b.Title.Length, n) 
          .Select(b => new ReportLine(b.Isbn, b.Title, b.Year, b.IsIssued))
          .ToList();
}





