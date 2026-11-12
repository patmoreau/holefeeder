using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Insights.Core;

internal sealed record InsightCategory(Id Id, string Name, string Color, decimal Budget);
