using DrifterApps.Seeds.FluentResult;

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
    bool Inactive)
{
    public Result<MyDataCashflow> Validated() =>
        MyDataRecordErrors.Validated(this,
            (EffectiveDate != default, MyDataRecordErrors.CashflowEffectiveDateRequired),
            (Frequency > 0, MyDataRecordErrors.CashflowFrequencyInvalid),
            (Recurrence >= 0, MyDataRecordErrors.CashflowRecurrenceInvalid));
}
