using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Insights.Core;

internal static class InsightsBreakdown
{
    public static async Task<Result<Breakdown>> BreakdownAsync(
        this IInsights insights, User user, CategoryType type, DateOnly today, CancellationToken cancellationToken)
    {
        var settingsResult = await insights.GetPeriodSettingsAsync(user, today, cancellationToken);
        if (settingsResult.IsFailure)
        {
            return settingsResult.Error;
        }

        var categoriesResult = await insights.GetCategoriesAsync(user, type, cancellationToken);
        if (categoriesResult.IsFailure)
        {
            return categoriesResult.Error;
        }

        var entriesResult = await insights.GetEntriesAsync(user, type, cancellationToken);
        if (entriesResult.IsFailure)
        {
            return entriesResult.Error;
        }

        return Breakdown.Of(categoriesResult.Value, entriesResult.Value, today, settingsResult.Value);
    }
}
