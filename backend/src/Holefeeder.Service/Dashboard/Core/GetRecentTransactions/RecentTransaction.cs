using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Dashboard.Core.GetRecentTransactions;

internal sealed record RecentTransaction(
    Id Id,
    DateOnly Date,
    decimal Amount,
    string Description,
    string CategoryName,
    string CategoryColor,
    IReadOnlyList<string> Tags);
