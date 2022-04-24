using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Domain.SeedWork;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.Application.Features.Cashflows.Exceptions;

public class CashflowNotFoundException : DomainException
{
    public CashflowNotFoundException(Guid id) : base(StatusCodes.Status404NotFound,
        $"{nameof(Cashflow)} '{id}' not found")
    {
    }

    public override string Context => nameof(CashflowNotFoundException);
}
