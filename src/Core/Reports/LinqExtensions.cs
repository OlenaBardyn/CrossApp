namespace Core.Reports;

public static class LinqExtensions
{
    public static IEnumerable<T> TopBy<T>(
        this IEnumerable<T> source,
        Func<T, int> selector,
        int n)
    {
        return source.OrderByDescending(selector)
                     .Take(n);
    }
}