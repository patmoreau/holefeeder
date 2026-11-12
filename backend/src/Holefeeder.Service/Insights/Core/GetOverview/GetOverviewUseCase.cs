using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Insights.Core.GetOverview;

internal sealed class GetOverviewUseCase(IInsights insights)
{
    public async Task<Result<Overview>> HandleAsync(GetOverviewRequest request, CancellationToken cancellationToken)
    {
        var settingsResult = await insights.GetPeriodSettingsAsync(request.User, request.Today, cancellationToken);
        if (settingsResult.IsFailure)
        {
            return settingsResult.Error;
        }

        var incomeResult = await insights.GetEntriesAsync(request.User, CategoryType.Gain, cancellationToken);
        if (incomeResult.IsFailure)
        {
            return incomeResult.Error;
        }

        var spendingResult = await insights.GetEntriesAsync(request.User, CategoryType.Expense, cancellationToken);
        if (spendingResult.IsFailure)
        {
            return spendingResult.Error;
        }

        return Overview.Of(incomeResult.Value, spendingResult.Value, request.Today, settingsResult.Value, request.Periods);
    }
}
