using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Dashboard.Core.GetRecentTransactions;

internal sealed record DashboardTransaction(
    Id Id,
    string Title,
    string? Category,
    string CategoryColor,
    IReadOnlyList<string> Tags,
    decimal Amount,
    DateOnly Date);
