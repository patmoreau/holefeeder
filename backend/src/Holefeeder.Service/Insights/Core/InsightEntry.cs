using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Insights.Core;

internal sealed record InsightEntry(DateOnly Date, Id CategoryId, decimal Amount, string Tags);
