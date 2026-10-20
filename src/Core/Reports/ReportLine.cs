namespace Core.Reports;
public sealed record ReportLine(string Isbn, string Title, int Year, bool IsIssued);

//додаткове 1
public sealed record LoanReport(string Isbn, string Title, string ReaderId);