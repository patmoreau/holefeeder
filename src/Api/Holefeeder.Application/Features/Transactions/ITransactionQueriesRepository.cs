using Holefeeder.Application.Models;
using Holefeeder.Application.SeedWork;

namespace Holefeeder.Application.Features.Transactions;

public interface ITransactionQueriesRepository
{
    Task<(int Total, IEnumerable<TransactionInfoViewModel> Items)> FindAsync(Guid userId, QueryParams queryParams,
        CancellationToken cancellationToken);

    Task<TransactionInfoViewModel?> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken);
}
