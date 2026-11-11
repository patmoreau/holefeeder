using Holefeeder.Service.Dashboard.Core;
using Holefeeder.Service.Dashboard.Core.GetDashboardSummary;

namespace Holefeeder.Service.Dashboard.Persistence;

internal sealed record CashflowRow(
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
            EffectiveDate,
            Core.IntervalType.Valid(IntervalType),
            Frequency,
            Amount,
            Enum.Parse<CategoryType>(CategoryType, ignoreCase: true),
            LastPaidDate,
            LastCashflowDate);
}
