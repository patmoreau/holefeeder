using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Insights.Core.GetSpending;

internal sealed class GetSpendingUseCase(IInsights insights)
{
    public Task<Result<Breakdown>> HandleAsync(GetSpendingRequest request, CancellationToken cancellationToken) =>
        insights.BreakdownAsync(request.User, CategoryType.Expense, request.Today, cancellationToken);
}
