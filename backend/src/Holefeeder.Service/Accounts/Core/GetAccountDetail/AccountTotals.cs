using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Accounts.Core.GetAccountDetail;

internal sealed record AccountTotals(
    Id Id,
    string Name,
    AccountType Type,
    string Description,
    decimal OpenBalance,
    DateOnly OpenDate,
    bool Favorite,
    bool Inactive,
    decimal Gains,
    decimal Expenses,
    DateOnly? LastTransactionDate);
