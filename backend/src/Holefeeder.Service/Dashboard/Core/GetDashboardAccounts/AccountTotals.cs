using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Dashboard.Core.GetDashboardAccounts;

internal sealed record AccountTotals(
    Id Id,
    string Name,
    AccountType Type,
    decimal OpenBalance,
    DateOnly OpenDate,
    decimal Gains,
    decimal Expenses,
    DateOnly? LastTransactionDate);
