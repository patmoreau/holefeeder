using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Insights.Core.GetOverview;

internal enum NetOutcome
{
    None,
    Saved,
    Overspent,
    BrokeEven
}

internal sealed record PeriodFlow(Period Period, decimal Income, decimal Spending);

internal sealed record Overview(
    Period Period,
    decimal Income,
    decimal Spending,
    decimal Net,
    NetOutcome Outcome,
    int SavedPercentage,
    IReadOnlyList<PeriodFlow> Series,
    decimal TotalIncome,
    decimal TotalSpending,
    decimal TotalNet,
    RangeSpan Range)
{
    public static Overview Of(
        IReadOnlyList<InsightEntry> income, IReadOnlyList<InsightEntry> spending, DateOnly today, PeriodSettings settings, PeriodCount count)
    {
        var current = Period.Containing(today, settings);
        var currentIncome = SumWithin(income, current);
        var currentSpending = SumWithin(spending, current);
        var net = currentIncome - currentSpending;
        var outcome = OutcomeOf(currentIncome, currentSpending, net);
        var savedPercentage = outcome == NetOutcome.Saved ? (int)Math.Round(net / currentIncome * 100, MidpointRounding.AwayFromZero) : 0;

        var series = PeriodsEndingWith(current, settings, count.Value)
            .Select(period => new PeriodFlow(period, SumWithin(income, period), SumWithin(spending, period)))
            .ToList();
        var totalIncome = series.Sum(flow => flow.Income);
        var totalSpending = series.Sum(flow => flow.Spending);

        return new Overview(
            current,
            currentIncome,
            currentSpending,
            net,
            outcome,
            savedPercentage,
            series,
            totalIncome,
            totalSpending,
            totalIncome - totalSpending,
            RangeSpan.Of(count.Value, settings));
    }

    private static NetOutcome OutcomeOf(decimal income, decimal spending, decimal net) =>
        (income, spending, net) switch
        {
            (0, 0, _) => NetOutcome.None,
            (_, _, > 0) => NetOutcome.Saved,
            (_, _, < 0) => NetOutcome.Overspent,
            _ => NetOutcome.BrokeEven
        };

    private static List<Period> PeriodsEndingWith(Period current, PeriodSettings settings, int count)
    {
        var stepping = settings.IntervalType == IntervalType.OneTime
            ? PeriodSettings.Valid(current.Start, IntervalType.Daily, 1)
            : settings;
        return Enumerable.Range(0, count)
            .Select(index => Period.Containing(
                stepping.IntervalType.AddIteration(current.Start, -(count - 1 - index) * stepping.Frequency), stepping))
            .ToList();
    }

    private static decimal SumWithin(IEnumerable<InsightEntry> entries, Period period) =>
        entries.Where(entry => entry.Date >= period.Start && entry.Date <= period.End).Sum(entry => entry.Amount);
}
