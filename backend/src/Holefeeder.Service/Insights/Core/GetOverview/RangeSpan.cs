using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Insights.Core.GetOverview;

internal enum RangeUnit
{
    Day,
    Week,
    Month,
    Year
}

internal sealed record RangeSpan(RangeUnit Unit, int Amount)
{
    public static RangeSpan Of(int count, PeriodSettings settings)
    {
        var amount = count * settings.Frequency;
        return settings.IntervalType.Name switch
        {
            nameof(IntervalType.Monthly) => InYearsWhenWhole(amount, 12, RangeUnit.Month),
            nameof(IntervalType.Weekly) => InYearsWhenWhole(amount, 52, RangeUnit.Week),
            nameof(IntervalType.Yearly) => new RangeSpan(RangeUnit.Year, amount),
            _ => new RangeSpan(RangeUnit.Day, amount)
        };
    }

    private static RangeSpan InYearsWhenWhole(int amount, int perYear, RangeUnit unit) =>
        amount % perYear == 0 ? new RangeSpan(RangeUnit.Year, amount / perYear) : new RangeSpan(unit, amount);
}
