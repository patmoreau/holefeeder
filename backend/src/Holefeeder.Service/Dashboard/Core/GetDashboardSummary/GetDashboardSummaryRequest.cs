using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Dashboard.Core.GetDashboardSummary;

internal record GetDashboardSummaryRequest(User User, DateOnly Today);
