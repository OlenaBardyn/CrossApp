using Core.Abstractions;
using Core.Storage;

namespace Core;

public static class StoreFactory
{
    public static IBookStore Create(string[] args, string filePath)
    {
        bool useFile = args.Contains("--file");

        return useFile
            ? new FileBookStore(filePath)
            : new InMemoryBookStore(SampleData.Books());
    }
}