namespace Core.Domain;

public sealed class Loan
{
    private Loan(string id, BookCopy copy, string readerId, DateTime issuedOn)
    {
        Id = id;
        Copy = copy;
        ReaderId = readerId;
        IssuedOn = issuedOn;
        ReturnedOn = null;
    }

    public string Id { get; }
    public BookCopy Copy { get; }
    public string ReaderId { get; }
    public DateTime IssuedOn { get; }
    public DateTime? ReturnedOn { get; private set; }
    public LoanStatus Status { get; private set; } //додаткове 3
    //public bool IsClosed => ReturnedOn is not null;
    public bool IsClosed => Status == LoanStatus.Returned || Status == LoanStatus.Cancelled; //

    // фабричний метод
    public static Loan Open(string id, BookCopy copy, string readerId, DateTime issuedOn)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Ідентифікатор обов'язковий", nameof(id));

        if (copy is null)
            throw new ArgumentNullException(nameof(copy));

        if (string.IsNullOrWhiteSpace(readerId))
            throw new ArgumentException("Ідентифікатор читача обов'язковий", nameof(readerId));

        if (copy.IsIssued)
            throw new InvalidOperationException(
                $"Примірник {copy.Id} вже виданий, відкрити видачу неможливо");

        copy.Issue();

        //return new Loan(id.Trim(), copy, readerId.Trim(), issuedOn);
        Loan loan = new(id.Trim(), copy, readerId.Trim(), issuedOn);
        loan.Status = LoanStatus.Active;
        return loan;
    }


    public void Close(DateTime returnedOn)
    {
        //if (IsClosed)
         //   throw new InvalidOperationException(
           //     $"Видача {Id} вже закрита, повторне закриття неможливе");

        EnsureTransition(LoanStatus.Returned); ////

        if (returnedOn < IssuedOn)
            throw new ArgumentOutOfRangeException(
                nameof(returnedOn), returnedOn,
                "Дата повернення не може бути раніше дати видачі");

        ReturnedOn = returnedOn;

        Status = LoanStatus.Returned; ////

        Copy.Return();
    }

    public void Cancel() ////////////////
    {
        EnsureTransition(LoanStatus.Cancelled);
        Status = LoanStatus.Cancelled;
        Copy.Return();
    }

    private void EnsureTransition(LoanStatus target)
    {
        bool allowed = (Status, target) switch
        {
            (LoanStatus.Active, LoanStatus.Returned) => true,
            (LoanStatus.Active, LoanStatus.Cancelled) => true,
            _ => false
        };

        if (!allowed)
            throw new InvalidOperationException(
                $"Перехід {Status} → {target} неможливий для видачі {Id}");
    }///////////////

    public override string ToString() =>
        $"{Id}: {Copy.Id} → читач {ReaderId}, видано {IssuedOn:yyyy-MM-dd}" +
        (IsClosed ? $", повернено {ReturnedOn:yyyy-MM-dd}" : ", відкрита");
}