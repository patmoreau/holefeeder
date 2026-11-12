using Holefeeder.Service.Insights.Core;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Insights.Persistence;

internal sealed record InsightCategoryRow(Guid Id, string Name, string Color, decimal BudgetAmount)
{
    public InsightCategory ToInsightCategory() => new(Shared.Core.Id.Valid(Id), Name, Color, BudgetAmount);
}
