using Holefeeder.Domain.SeedWork;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.Domain.Features.Transactions;

public class TransactionDomainException : DomainException
{
    public TransactionDomainException(string message, string context) : base(StatusCodes.Status422UnprocessableEntity, message)
    {
        Context = context;
    }

    public override string Context { get; }
}
