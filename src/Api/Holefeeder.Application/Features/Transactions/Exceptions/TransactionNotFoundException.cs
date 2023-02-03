using Holefeeder.Application.Exceptions;
using Holefeeder.Domain.Features.Transactions;

namespace Holefeeder.Application.Features.Transactions.Exceptions;

#pragma warning disable CA1032
public class TransactionNotFoundException : NotFoundException<Transaction>
#pragma warning restore CA1032
{
    public TransactionNotFoundException(Guid id) : base(id)
    {
    }
}
