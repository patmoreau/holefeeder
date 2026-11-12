namespace Holefeeder.Service.Shared.Core;

internal static class PeriodTotals
{
    public static Dictionary<Period, decimal> ByPeriod(IEnumerable<(DateOnly Date, decimal Amount)> amounts, Period current, PeriodSettings settings)
    {
        var periods = PeriodsAnchoredOn(current, settings);
        return amounts
            .GroupBy(amount => Period.Containing(amount.Date, periods))
            .ToDictionary(group => group.Key, group => group.Sum(amount => amount.Amount));
    }

    public static decimal AverageOf(IReadOnlyDictionary<Period, decimal> totals) =>
        totals.Count == 0 ? 0 : Math.Round(totals.Values.Sum() / totals.Count, 2);

    private static PeriodSettings PeriodsAnchoredOn(Period current, PeriodSettings settings) =>
        settings.IntervalType == IntervalType.OneTime
            ? PeriodSettings.Valid(current.Start, IntervalType.Daily, 1)
            : PeriodSettings.Valid(current.Start, settings.IntervalType, settings.Frequency);
}
