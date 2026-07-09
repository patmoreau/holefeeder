using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Users;

namespace Holefeeder.Application.Features.Transactions;

public interface IUpcomingQueriesRepository
{
    Task<IEnumerable<UpcomingViewModel>> GetUpcomingAsync(UserId userId, DateOnly startDate, DateOnly endDate,
        CancellationToken cancellationToken = default);
}
