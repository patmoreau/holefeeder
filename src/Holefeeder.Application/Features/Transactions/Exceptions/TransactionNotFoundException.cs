using Holefeeder.Application.Exceptions;
using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Application.Features.Transactions.Exceptions;

#pragma warning disable CA1032
public class TransactionNotFoundException(Guid id) : NotFoundException<Transaction>(id)
#pragma warning restore CA1032
{
}
