using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.MyData.Core;

internal sealed record MyDataCashflow(
    Id Id,
    DateOnly EffectiveDate,
    Money Amount,
    IntervalType IntervalType,
    int Frequency,
    int Recurrence,
    string Description,
    TagList Tags,
    Id CategoryId,
    Id AccountId,
    bool Inactive);
