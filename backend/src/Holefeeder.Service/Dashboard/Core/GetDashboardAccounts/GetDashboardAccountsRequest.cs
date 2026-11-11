using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Dashboard.Core.GetDashboardAccounts;

internal record GetDashboardAccountsRequest(User User, DateOnly Today);
