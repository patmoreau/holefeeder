using Holefeeder.Application.SeedWork;

namespace Holefeeder.Application.Features.Accounts.Queries;

public interface IAccountQueriesRepository
{
    Task<(int Total, IEnumerable<AccountViewModel> Items)> FindAsync(Guid userId, QueryParams queryParams,
        CancellationToken cancellationToken);

    Task<AccountViewModel?> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken);

    Task<bool> IsAccountActive(Guid id, Guid userId, CancellationToken cancellationToken);
}
