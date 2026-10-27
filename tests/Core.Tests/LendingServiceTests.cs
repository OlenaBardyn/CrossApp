using Core.Domain;
using Core.Events;
using Core.Services;
using Core.Storage;

namespace Core.Tests;

public class LendingServiceTests
{
    [Fact]
    public void Issue_WhenAlreadyIssued_Throws_AndKeepsState()
    {
        var store = new InMemoryBookStore();
        var service = new LendingService(store);
        var book = service.AddBook("978-1", "Книга", 1840);

        service.IssueCopy(book.Id, "R-1");

        var ex = Assert.Throws<InvalidOperationException>(() => service.IssueCopy(book.Id, "R-2"));
        Assert.Contains("вже виданий", ex.Message);
        Assert.True(store.GetById(book.Id)!.IsIssued);
    }

    [Fact]
    public void Add_RaisesChangedExactlyOnce()
    {
        var service = new LendingService(new InMemoryBookStore());
        var events = new List<BookChangedEventArgs>();
        service.Changed += (_, e) => events.Add(e);

        service.AddBook("978-1", "Книга", 1840);

        Assert.Single(events);
        Assert.Equal(BookChangeKind.Added, events[0].Kind);
    }
}