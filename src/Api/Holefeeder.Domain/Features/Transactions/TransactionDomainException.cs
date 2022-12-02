using Microsoft.AspNetCore.Http;

namespace Holefeeder.Domain.Features.Transactions;

#pragma warning disable CA1032
public class TransactionDomainException : DomainException<Transaction>
#pragma warning restore CA1032
{
    public TransactionDomainException(string message, string context) : base(message)
    {
        Context = context;
    }

    public TransactionDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public TransactionDomainException(string message) : base(message)
    {
    }

    public override string Context { get; } = nameof(Transaction);
}
