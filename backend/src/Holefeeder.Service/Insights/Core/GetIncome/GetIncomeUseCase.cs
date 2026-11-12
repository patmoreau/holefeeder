using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Insights.Core.GetIncome;

internal sealed class GetIncomeUseCase(IInsights insights)
{
    public Task<Result<Breakdown>> HandleAsync(GetIncomeRequest request, CancellationToken cancellationToken) =>
        insights.BreakdownAsync(request.User, CategoryType.Gain, request.Today, cancellationToken);
}
