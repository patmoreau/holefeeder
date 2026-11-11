using Holefeeder.Service.Accounts.Core.GetAccountTransactions;

namespace Holefeeder.Service.Accounts.Persistence;

internal sealed record RecordedTransactionRow(
    Guid Id,
    Guid AccountId,
    DateOnly Date,
    decimal Amount,
    string? Description,
    string CategoryName,
    string CategoryColor,
    string? Tags)
{
    public RecordedTransaction ToRecordedTransaction() =>
        new(
            Shared.Core.Id.Valid(Id),
            Shared.Core.Id.Valid(AccountId),
            Date,
            Amount,
            Description ?? string.Empty,
            CategoryName,
            CategoryColor,
            (Tags ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
}
