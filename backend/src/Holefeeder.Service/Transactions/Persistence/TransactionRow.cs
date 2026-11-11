using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Transactions.Core;

namespace Holefeeder.Service.Transactions.Persistence;

internal sealed record TransactionRow(
    Guid Id,
    DateOnly Date,
    decimal Amount,
    string? Description,
    Guid AccountId,
    Guid CategoryId,
    string? Tags,
    Guid? CashflowId,
    DateOnly? CashflowDate)
{
    public Transaction ToTransaction() =>
        new(
            Shared.Core.Id.Valid(Id),
            Date,
            Money.Valid(Amount),
            Description ?? string.Empty,
            Shared.Core.Id.Valid(AccountId),
            Shared.Core.Id.Valid(CategoryId),
            Core.Tags.Create((Tags ?? string.Empty).Split(',')),
            CashflowId is { } cashflowId ? Shared.Core.Id.Valid(cashflowId) : null,
            CashflowDate);
}
