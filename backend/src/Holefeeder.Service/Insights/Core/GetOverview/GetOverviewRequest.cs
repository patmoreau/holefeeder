using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Insights.Core.GetOverview;

internal sealed record GetOverviewRequest(User User, DateOnly Today, PeriodCount Periods);
