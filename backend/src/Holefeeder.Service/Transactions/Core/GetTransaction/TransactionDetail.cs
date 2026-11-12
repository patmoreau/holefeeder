using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Transactions.Core.GetTransaction;

internal sealed record TransactionDetail(
    Id Id,
    DateOnly Date,
    Money Amount,
    string Description,
    TagList Tags,
    TransactionAccount Account,
    TransactionCategory Category);
