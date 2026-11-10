namespace Api.Contracts;

public sealed record SaveBookRequest(string Isbn, string Title, int Year)
{
    public bool TryValidate(out Dictionary<string, string[]> errors)
    {
        var found = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(Isbn))
            found[nameof(Isbn)] = ["ISBN обов'язковий."];

        if (string.IsNullOrWhiteSpace(Title) || Title.Length > 200)
            found[nameof(Title)] = ["Назва обов'язкова, до 200 символів."];

        if (Year < 1450 || Year > DateTime.Now.Year)
        if (Year < 1450 || Year > DateTime.Now.Year)
            found[nameof(Year)] = [$"Рік має бути від 1450 до {DateTime.Now.Year}."];

        errors = found;
        return found.Count == 0;
    }
}

public sealed record PagedResult<T>(List<T> Items, int Total);