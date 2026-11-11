namespace Holefeeder.Service.Accounts.Api.GetAccountTransactions;

internal sealed record AccountTransactionResponse(
    Guid Id,
    string Title,
    string? Category,
    string CategoryColor,
    IReadOnlyList<string> Tags,
    decimal Amount,
    DateOnly Date);
