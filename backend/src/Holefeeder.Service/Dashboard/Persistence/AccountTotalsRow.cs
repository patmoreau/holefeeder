using Holefeeder.Service.Dashboard.Core.GetDashboardAccounts;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Dashboard.Persistence;

internal sealed record AccountTotalsRow(
    Guid Id,
    string Name,
    string Type,
    decimal OpenBalance,
    DateOnly OpenDate,
    decimal Gains,
    decimal Expenses,
    DateOnly? LastTransactionDate)
{
    public AccountTotals ToAccountTotals() =>
        new(
            Shared.Core.Id.Valid(Id),
            Name,
            AccountType.Valid(Type),
            OpenBalance,
            OpenDate,
            Gains,
            Expenses,
            LastTransactionDate);
}
