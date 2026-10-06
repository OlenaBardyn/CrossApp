using Core.Abstractions;
using Core.Domain;

namespace Core.Storage;

public sealed class CachingBookStore(IBookStore inner) : IBookStore
{
    private readonly Dictionary<string, BookCopy> _cache = new(StringComparer.OrdinalIgnoreCase);
    private bool _loaded;

    private void EnsureLoaded()
    {
        if (_loaded) return;
        foreach (var item in inner.List())
            _cache[item.Id] = item;
        _loaded = true;
    }

    public IReadOnlyList<BookCopy> List()
    {
        EnsureLoaded();
        return _cache.Values.ToList();
    }

    public BookCopy? GetById(string id)
    {
        EnsureLoaded();
        return _cache.GetValueOrDefault(id);
    }

    public void Add(BookCopy item)
    {
        ArgumentNullException.ThrowIfNull(item);
        EnsureLoaded();
        if (_cache.ContainsKey(item.Id))
            throw new InvalidOperationException($"Запис з id={item.Id} уже існує.");
        inner.Add(item);
        _cache[item.Id] = item;
    }

    public void Update(BookCopy item)
    {
        EnsureLoaded();
        inner.Update(item);
        _cache[item.Id] = item;
    }

    public bool Remove(string id)
    {
        EnsureLoaded();
        bool removed = inner.Remove(id);
        if (removed) _cache.Remove(id);
        return removed;
    }
}