namespace Core.Events;

public sealed class BookChangedEventArgs(
    string id,
    BookChangeKind kind,
    string? readerId,
    bool isIssuedAfter) : EventArgs
{
    public string Id { get; } = id;
    public BookChangeKind Kind { get; } = kind;
    public string? ReaderId { get; } = readerId;
    public bool IsIssuedAfter { get; } = isIssuedAfter;
    public DateTimeOffset At { get; } = DateTimeOffset.Now;
}