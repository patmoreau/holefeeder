using Holefeeder.Service.Insights.Core;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Insights.Persistence;

internal sealed record InsightEntryRow(DateOnly Date, Guid CategoryId, decimal Amount, string Tags)
{
    public InsightEntry ToInsightEntry() => new(Date, Id.Valid(CategoryId), Amount, Tags);
}
