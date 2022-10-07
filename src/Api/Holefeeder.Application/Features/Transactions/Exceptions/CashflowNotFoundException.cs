using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Domain.SeedWork;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.Application.Features.Transactions.Exceptions;

public class CashflowNotFoundException : DomainException
{
    public CashflowNotFoundException(Guid id) : base(StatusCodes.Status404NotFound,
        $"{nameof(Cashflow)} '{id}' not found")
    {
    }

    public CashflowNotFoundException()
    {
    }

    public CashflowNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public CashflowNotFoundException(string message) : base(message)
    {
    }

    public override string Context => nameof(Transactions);
}
