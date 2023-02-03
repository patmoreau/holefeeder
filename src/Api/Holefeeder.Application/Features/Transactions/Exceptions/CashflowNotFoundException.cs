using Holefeeder.Application.Exceptions;
using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Application.Features.Transactions.Exceptions;

#pragma warning disable CA1032
public class CashflowNotFoundException : NotFoundException<Cashflow>
#pragma warning restore CA1032
{
    public CashflowNotFoundException(Guid id) : base(id)
    {
    }
}
