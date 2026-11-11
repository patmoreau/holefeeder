using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Dashboard.Core.GetDashboardAccounts;

internal sealed record DashboardAccount(
    Id Id,
    string Name,
    BalanceCaption Caption,
    decimal Balance,
    decimal ProjectedBalance,
    DateOnly Updated);
