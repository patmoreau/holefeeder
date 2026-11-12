using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Insights.Core.GetSpending;

internal sealed class GetSpendingUseCase(IInsights insights)
{
    public async Task<Result<Breakdown>> HandleAsync(GetSpendingRequest request, CancellationToken cancellationToken)
    {
        var settingsResult = await insights.GetPeriodSettingsAsync(request.User, request.Today, cancellationToken);
        if (settingsResult.IsFailure)
        {
            return settingsResult.Error;
        }

        var categoriesResult = await insights.GetCategoriesAsync(request.User, CategoryType.Expense, cancellationToken);
        if (categoriesResult.IsFailure)
        {
            return categoriesResult.Error;
        }

        var entriesResult = await insights.GetEntriesAsync(request.User, CategoryType.Expense, cancellationToken);
        if (entriesResult.IsFailure)
        {
            return entriesResult.Error;
        }

        return Breakdown.Of(categoriesResult.Value, entriesResult.Value, request.Today, settingsResult.Value);
    }
}
