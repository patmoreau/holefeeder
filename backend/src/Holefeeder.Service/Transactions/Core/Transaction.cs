using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Transactions.Core;

internal sealed record Transaction(
    Id Id,
    DateOnly Date,
    Money Amount,
    string Description,
    Id AccountId,
    Id CategoryId,
    TagList Tags,
    Id? CashflowId,
    DateOnly? CashflowDate);
