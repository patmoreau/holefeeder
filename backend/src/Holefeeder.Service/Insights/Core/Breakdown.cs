using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Insights.Core;

internal sealed record Breakdown(
    Period Period,
    decimal Current,
    decimal Average,
    decimal Variation,
    decimal VariationPercentage,
    IReadOnlyList<CategoryBreakdown> Categories,
    IReadOnlyList<TagBreakdown> Tags)
{
    public static Breakdown Of(
        IReadOnlyList<InsightCategory> categories, IReadOnlyList<InsightEntry> entries, DateOnly today, PeriodSettings settings)
    {
        var current = Period.Containing(today, settings);
        var previous = PreviousPeriods(current, settings);
        var tagged = entries.SelectMany(entry => TagsOf(entry).Select(tag => (Entry: entry, Tag: tag))).ToList();

        var totals = PeriodTotals.ByPeriod(entries.Select(entry => (entry.Date, entry.Amount)), current, settings);
        var currentTotal = totals.GetValueOrDefault(current);
        var average = PeriodTotals.AverageOf(totals);
        var variation = currentTotal - average;
        var variationPercentage = average == 0 ? 0 : Math.Round(variation / average * 100, 2);

        var byCategory = categories
            .Select(category =>
            {
                var categoryEntries = entries.Where(entry => entry.CategoryId == category.Id).ToList();
                var tags = tagged
                    .Where(item => item.Entry.CategoryId == category.Id && Within(item.Entry.Date, current))
                    .GroupBy(item => item.Tag, StringComparer.Ordinal)
                    .Select(group => new TagAmount(group.Key, group.Sum(item => item.Entry.Amount)))
                    .OrderByDescending(tag => tag.Amount)
                    .ThenBy(tag => tag.Tag, StringComparer.Ordinal)
                    .ToList();
                var amount = SumWithin(categoryEntries.Select(entry => (entry.Date, entry.Amount)), current);
                return new CategoryBreakdown(
                    category.Id,
                    category.Name,
                    category.Color,
                    category.Budget,
                    BudgetStatusOf(amount, category.Budget),
                    category.Budget > 0 ? (int)Math.Round(amount / category.Budget * 100, MidpointRounding.AwayFromZero) : 0,
                    amount,
                    AverageOver(previous, categoryEntries.Select(entry => (entry.Date, entry.Amount))),
                    tags);
            })
            .OrderByDescending(category => category.Average)
            .ThenByDescending(category => category.Amount)
            .ThenBy(category => category.Name, StringComparer.Ordinal)
            .ToList();

        var byTag = tagged
            .Where(item => Within(item.Entry.Date, current) || previous.Any(period => Within(item.Entry.Date, period)))
            .GroupBy(item => item.Tag, StringComparer.Ordinal)
            .Select(group =>
            {
                var amounts = group.Select(item => (item.Entry.Date, item.Entry.Amount)).ToList();
                return new TagBreakdown(group.Key, SumWithin(amounts, current), AverageOver(previous, amounts));
            })
            .OrderByDescending(tag => tag.Average)
            .ThenByDescending(tag => tag.Amount)
            .ThenBy(tag => tag.Tag, StringComparer.Ordinal)
            .ToList();

        return new Breakdown(current, currentTotal, average, variation, variationPercentage, byCategory, byTag);
    }

    private static List<Period> PreviousPeriods(Period current, PeriodSettings settings)
    {
        var periods = new List<Period>();
        if (settings.IntervalType == IntervalType.OneTime)
        {
            return periods;
        }

        var start = settings.EffectiveDate;
        var iteration = 1;
        while (start < current.Start)
        {
            var next = settings.IntervalType.AddIteration(settings.EffectiveDate, settings.Frequency * iteration);
            periods.Add(new Period(start, next.AddDays(-1)));
            start = next;
            iteration++;
        }

        return periods;
    }

    private static decimal AverageOver(List<Period> previous, IEnumerable<(DateOnly Date, decimal Amount)> amounts)
    {
        var inPrevious = amounts
            .Select(amount => (Index: previous.FindIndex(period => Within(amount.Date, period)), amount.Amount))
            .Where(amount => amount.Index >= 0)
            .ToList();
        if (inPrevious.Count == 0)
        {
            return 0;
        }

        var activePeriods = previous.Count - inPrevious.Min(amount => amount.Index);
        return Math.Round(inPrevious.Sum(amount => amount.Amount) / activePeriods, 2, MidpointRounding.AwayFromZero);
    }

    private static BudgetStatus BudgetStatusOf(decimal amount, decimal budget) =>
        budget <= 0 ? BudgetStatus.None : amount > budget ? BudgetStatus.Over : BudgetStatus.Under;

    private static decimal SumWithin(IEnumerable<(DateOnly Date, decimal Amount)> amounts, Period period) =>
        amounts.Where(amount => Within(amount.Date, period)).Sum(amount => amount.Amount);

    private static bool Within(DateOnly date, Period period) => date >= period.Start && date <= period.End;

    private static IEnumerable<string> TagsOf(InsightEntry entry) =>
        entry.Tags.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Distinct(StringComparer.Ordinal);
}

internal enum BudgetStatus
{
    None,
    Under,
    Over
}

internal sealed record CategoryBreakdown(
    Id Id,
    string Name,
    string Color,
    decimal Budget,
    BudgetStatus BudgetStatus,
    int BudgetPercentage,
    decimal Amount,
    decimal Average,
    IReadOnlyList<TagAmount> Tags);

internal sealed record TagBreakdown(string Tag, decimal Amount, decimal Average);

internal sealed record TagAmount(string Tag, decimal Amount);
