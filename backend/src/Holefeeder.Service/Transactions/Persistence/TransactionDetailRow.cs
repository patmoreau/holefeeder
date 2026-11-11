using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Transactions.Core;
using Holefeeder.Service.Transactions.Core.GetTransaction;

namespace Holefeeder.Service.Transactions.Persistence;

internal sealed record TransactionDetailRow(
    Guid Id,
    DateOnly Date,
    decimal Amount,
    string? Description,
    string? Tags,
    Guid AccountId,
    string AccountName,
    bool AccountFavorite,
    Guid CategoryId,
    string CategoryName,
    string CategoryColor,
    string CategoryType,
    bool CategoryFavorite)
{
    public TransactionDetail ToTransactionDetail() =>
        new(
            Shared.Core.Id.Valid(Id),
            Date,
            Money.Valid(Amount),
            Description ?? string.Empty,
            Core.Tags.Create((Tags ?? string.Empty).Split(',')),
            new TransactionAccount(Shared.Core.Id.Valid(AccountId), AccountName, AccountFavorite),
            new TransactionCategory(
                Shared.Core.Id.Valid(CategoryId), CategoryName, CategoryColor,
                Enum.Parse<Shared.Core.CategoryType>(CategoryType, ignoreCase: true), CategoryFavorite));
}
