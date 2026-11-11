namespace Holefeeder.Service.Accounts.Api.GetAccountDetail;

internal sealed record AccountDetailResponse(
    Guid Id,
    string Name,
    string Type,
    string Description,
    decimal OpenBalance,
    DateOnly OpenDate,
    bool Favorite,
    bool Inactive,
    string Caption,
    decimal Balance,
    decimal Upcoming,
    decimal ProjectedBalance,
    DateOnly Updated);
