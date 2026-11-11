using Holefeeder.Service.Accounts.Core.GetAccountDetail;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Accounts.Persistence;

internal sealed record AccountTotalsRow(
    Guid Id,
    string Name,
    string Type,
    string? Description,
    decimal OpenBalance,
    DateOnly OpenDate,
    bool Favorite,
    bool Inactive,
    decimal Gains,
    decimal Expenses,
    DateOnly? LastTransactionDate)
{
    public AccountTotals ToAccountTotals() =>
        new(
            Shared.Core.Id.Valid(Id),
            Name,
            AccountType.Valid(Type),
            Description ?? string.Empty,
            OpenBalance,
            OpenDate,
            Favorite,
            Inactive,
            Gains,
            Expenses,
            LastTransactionDate);
}
