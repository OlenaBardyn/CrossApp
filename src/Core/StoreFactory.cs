using Core.Abstractions;
using Core.Storage;

namespace Core;

public static class StoreFactory
{
    public static IBookStore Create(string[] args)
    {
        bool useFile = args.Contains("--file");
        string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

        return useFile
            ? new FileBookStore(dataPath)
            : new InMemoryBookStore(SampleData.Books());
    }
}