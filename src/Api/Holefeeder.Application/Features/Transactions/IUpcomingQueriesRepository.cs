using Holefeeder.Application.Models;

namespace Holefeeder.Application.Features.Transactions;

public interface IUpcomingQueriesRepository
{
    Task<IEnumerable<UpcomingViewModel>> GetUpcomingAsync(Guid userId, DateOnly startDate, DateOnly endDate,
        CancellationToken cancellationToken = default);
}
