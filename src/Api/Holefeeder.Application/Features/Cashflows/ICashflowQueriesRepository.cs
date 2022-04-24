using Holefeeder.Application.Models;
using Holefeeder.Application.SeedWork;

namespace Holefeeder.Application.Features.Cashflows;

public interface ICashflowQueriesRepository
{
    Task<(int Total, IEnumerable<CashflowInfoViewModel> Items)> FindAsync(Guid userId, QueryParams queryParams,
        CancellationToken cancellationToken);

    Task<CashflowInfoViewModel?> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken);
}
