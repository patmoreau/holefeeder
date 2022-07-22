using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Domain.SeedWork;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.Application.Features.Transactions.Exceptions;

public class TransactionNotFoundException : DomainException
{
    public TransactionNotFoundException(Guid id) : base(StatusCodes.Status404NotFound,
        $"{nameof(Transaction)} '{id}' not found")
    {
    }

    public override string Context => nameof(Transactions);
}
