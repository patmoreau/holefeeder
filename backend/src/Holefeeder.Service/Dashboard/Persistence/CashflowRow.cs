using Holefeeder.Service.Dashboard.Core;

namespace Holefeeder.Service.Dashboard.Persistence;

internal sealed record CashflowRow(
    Guid AccountId,
    DateOnly EffectiveDate,
    string IntervalType,
    int Frequency,
    decimal Amount,
    string CategoryType,
    DateOnly? LastPaidDate,
    DateOnly? LastCashflowDate)
{
    public Cashflow ToCashflow() =>
        new(
            Shared.Core.Id.Valid(AccountId),
            EffectiveDate,
            Core.IntervalType.Valid(IntervalType),
            Frequency,
            Amount,
            Enum.Parse<CategoryType>(CategoryType, ignoreCase: true),
            LastPaidDate,
            LastCashflowDate);
}
