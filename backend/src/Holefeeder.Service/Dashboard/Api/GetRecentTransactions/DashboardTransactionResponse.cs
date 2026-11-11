namespace Holefeeder.Service.Dashboard.Api.GetRecentTransactions;

internal sealed record DashboardTransactionResponse(
    Guid Id,
    string Title,
    string? Category,
    string CategoryColor,
    IReadOnlyList<string> Tags,
    decimal Amount,
    DateOnly Date);
