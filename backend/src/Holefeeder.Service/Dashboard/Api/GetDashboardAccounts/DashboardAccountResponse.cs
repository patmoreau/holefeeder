namespace Holefeeder.Service.Dashboard.Api.GetDashboardAccounts;

internal sealed record DashboardAccountResponse(
    Guid Id,
    string Name,
    string Caption,
    decimal Balance,
    decimal ProjectedBalance,
    DateOnly Updated);
