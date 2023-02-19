using Holefeeder.Application.Models;

namespace Holefeeder.Application.Features.Transactions;

public interface IUpcomingQueriesRepository
{
    Task<IEnumerable<UpcomingViewModel>> GetUpcomingAsync(Guid userId, DateTime startDate, DateTime endDate,
        CancellationToken cancellationToken = default);
}
