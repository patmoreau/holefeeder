using Holefeeder.Application.Exceptions;
using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Application.Features.Transactions.Exceptions;

#pragma warning disable CA1032
public class CashflowNotFoundException(Guid id) : NotFoundException<Cashflow>(id)
#pragma warning restore CA1032
{
}
