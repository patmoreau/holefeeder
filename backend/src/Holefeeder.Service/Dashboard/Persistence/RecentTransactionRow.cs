using Holefeeder.Service.Dashboard.Core.GetRecentTransactions;

namespace Holefeeder.Service.Dashboard.Persistence;

internal sealed record RecentTransactionRow(
    Guid Id,
    DateOnly Date,
    decimal Amount,
    string? Description,
    string CategoryName,
    string CategoryColor,
    string? Tags)
{
    public RecentTransaction ToRecentTransaction() =>
        new(
            Shared.Core.Id.Valid(Id),
            Date,
            Amount,
            Description ?? string.Empty,
            CategoryName,
            CategoryColor,
            (Tags ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
}
