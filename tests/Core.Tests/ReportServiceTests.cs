using Core.Reports;

namespace Core.Tests;

public class ReportServiceTests
{
    [Theory]
    [InlineData(1900, 1)]
    [InlineData(1950, 3)]
    public void OldBooks_CountsOnlyBelowThreshold(int threshold, int expected)
    {
        var result = ReportService.OldBooks(TestData.Books(), threshold);
        Assert.Equal(expected, result.Count);
    }

    [Fact]
    public void NewestBooks_IsDeterministic()
    {
        var result = ReportService.NewestBooks(TestData.Books(), 2).Select(l => l.Title).ToList();
        Assert.Equal(new[] { "Книга 3", "Книга 2" }, result);
    }
}