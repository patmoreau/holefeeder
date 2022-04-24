using Holefeeder.Domain.SeedWork;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.Domain.Features.Transactions;

public class TransactionDomainException : DomainException
{
    public TransactionDomainException(string message) : base(StatusCodes.Status422UnprocessableEntity, message)
    {
    }

    public override string Context => nameof(Transaction);
}
