using System.Text.Json;
using Core.Abstractions;
using Core.Domain;
using Core.Dto;

namespace Core.Storage;

public sealed class JsonBookImporter : IBookImporter
{
    public async Task<IReadOnlyList<BookCopy>> LoadAsync(string path, CancellationToken ct = default)
    {
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read,
            FileShare.Read, 4096, useAsync: true);

        var dtos = await JsonSerializer.DeserializeAsync<List<BookDto>>(
            stream, cancellationToken: ct)
            ?? throw new DomainException($"Файл {Path.GetFileName(path)} не містить списку.");

        var result = new List<BookCopy>(dtos.Count);
        for (var i = 0; i < dtos.Count; i++)
        {
            ct.ThrowIfCancellationRequested();
            try { result.Add(BookCopy.FromDto(dtos[i])); }
            catch (ArgumentException ex)
            { throw new DomainException($"Запис {i + 1}: {ex.Message}", ex); }
        }
        return result;
    }


    //1
    public async IAsyncEnumerable<BookCopy> LoadStreamAsync(
    string path,
    [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        var dtos = await JsonSerializer.DeserializeAsync<List<BookDto>>(stream, cancellationToken: ct) ?? new();

        for (var i = 0; i < dtos.Count; i++)
        {
            ct.ThrowIfCancellationRequested();
            yield return BookCopy.FromDto(dtos[i]);
            await Task.Delay(20); 
        }
    }
}