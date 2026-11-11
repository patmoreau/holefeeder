using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Accounts.Core.GetAccountTransactions;

internal sealed record AccountTransaction(
    Id Id,
    string Title,
    string? Category,
    string CategoryColor,
    IReadOnlyList<string> Tags,
    decimal Amount,
    DateOnly Date);
