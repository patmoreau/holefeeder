using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Accounts.Persistence;

internal sealed record CashflowScheduleRow(
    Guid AccountId,
    DateOnly EffectiveDate,
    string IntervalType,
    int Frequency,
    decimal Amount,
    string CategoryType,
    DateOnly? LastPaidDate,
    DateOnly? LastCashflowDate)
{
    public CashflowSchedule ToCashflowSchedule() =>
        new(
            Id.Valid(AccountId),
            EffectiveDate,
            Shared.Core.IntervalType.Valid(IntervalType),
            Frequency,
            Amount,
            Enum.Parse<CategoryType>(CategoryType, ignoreCase: true),
            LastPaidDate,
            LastCashflowDate);
}
