using Core.Services;
using Core.Storage;

namespace Core.Tests;

public class ImportTests
{
    [Fact]
    public async Task ImportAsync_WhenAlreadyCancelled_Throws()
    {
        string tempFile = Path.GetTempFileName();
        await File.WriteAllTextAsync(tempFile, "[]");

        try
        {
            using var cts = new CancellationTokenSource();
            await cts.CancelAsync();
            var service = new LendingService(new InMemoryBookStore());

            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => service.ImportAsync(new JsonBookImporter(), tempFile, cts.Token));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ImportAsync_WhenTimeoutOccurs_Throws()
    {
        string tempFile = Path.GetTempFileName();
        string json = @"[{ ""Id"": ""J-1"", ""Isbn"": ""978-J-1"", ""Title"": ""Книга"", ""Year"": 2000 }]";
        await File.WriteAllTextAsync(tempFile, json);

        try
        {
            var service = new LendingService(new InMemoryBookStore());

            // таймаут 1 мілісекунда
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1));

            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => service.ImportAsync(new JsonBookImporter(), tempFile, timeoutCts.Token));

            Assert.True(timeoutCts.IsCancellationRequested);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}