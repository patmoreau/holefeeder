using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Accounts.Core.GetAccountDetail;

internal sealed record AccountDetail(
    Id Id,
    string Name,
    AccountType Type,
    string Description,
    decimal OpenBalance,
    DateOnly OpenDate,
    bool Favorite,
    bool Inactive,
    BalanceCaption Caption,
    decimal Balance,
    decimal Upcoming,
    decimal ProjectedBalance,
    DateOnly Updated);
