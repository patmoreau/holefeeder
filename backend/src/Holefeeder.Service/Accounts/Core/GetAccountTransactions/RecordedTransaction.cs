using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Accounts.Core.GetAccountTransactions;

internal sealed record RecordedTransaction(
    Id Id,
    Id AccountId,
    DateOnly Date,
    decimal Amount,
    string Description,
    string CategoryName,
    string CategoryColor,
    IReadOnlyList<string> Tags);
