using System.Net;
using System.Net.Http.Json;
using Web.Contracts;

namespace Web.Services;

public sealed class BooksApiClient(HttpClient http)
{
    public async Task<List<BookModel>> ListAsync(CancellationToken ct = default) =>
        await http.GetFromJsonAsync<List<BookModel>>("api/books", ct) ?? [];

    public Task<ApiResult> CreateAsync(BookModel m, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Post, "api/books", m, ct);

    public Task<ApiResult> UpdateAsync(BookModel m, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Put, $"api/books/{m.Id}", m, ct);

    //
    public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"api/books/{id}", ct);
        return response.IsSuccessStatusCode;
    }

    //
    public async Task<List<BookModel>> SearchAsync(string query, CancellationToken ct = default) =>
    await http.GetFromJsonAsync<List<BookModel>>($"api/books/search?query={query}", ct) ?? [];

    private async Task<ApiResult> SendAsync(HttpMethod method, string url, BookModel model, CancellationToken ct)
    {
        try
        {
            using var request = new HttpRequestMessage(method, url) { Content = JsonContent.Create(model) };
            using var response = await http.SendAsync(request, ct);

            if (response.IsSuccessStatusCode) return new ApiResult(true);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new ApiResult(false, "Запис не знайдено.");

            if (response.StatusCode != HttpStatusCode.BadRequest)
                return new ApiResult(false, $"Сервер повернув {(int)response.StatusCode}.");

            var problem = await response.Content.ReadFromJsonAsync<ValidationProblemResponse>(ct);
            return new ApiResult(false, problem?.Title ?? "Дані не пройшли перевірку.", problem?.Errors);
        }
        catch (HttpRequestException)
        {
            return new ApiResult(false, "Api недоступний. Перевірте, чи запущено src/Api.");
        }
    }
}