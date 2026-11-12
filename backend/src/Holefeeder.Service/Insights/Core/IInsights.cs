using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Insights.Core;

internal interface IInsights
{
    Task<Result<PeriodSettings>> GetPeriodSettingsAsync(User user, DateOnly today, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<InsightCategory>>> GetCategoriesAsync(User user, CategoryType type, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<InsightEntry>>> GetEntriesAsync(User user, CategoryType type, CancellationToken cancellationToken);
}
