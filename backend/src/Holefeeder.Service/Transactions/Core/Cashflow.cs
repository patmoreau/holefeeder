using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Transactions.Core;

internal sealed record Cashflow(
    Id Id,
    DateOnly EffectiveDate,
    Money Amount,
    IntervalType IntervalType,
    Frequency Frequency,
    string Description,
    Id AccountId,
    Id CategoryId,
    TagList Tags);
