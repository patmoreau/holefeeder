using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Domain.UseCases.Dashboard;

public static class SummaryCalculator
{
    public static SummaryResult Calculate(
        IEnumerable<SummaryData> data,
        DateInterval dateInterval,
        DateIntervalType intervalType,
        int frequency)
    {
        var groupedByTypeAndPeriod = GroupByTypeAndPeriod(data, dateInterval.Start, intervalType, frequency).ToList();

        var gains = FilterByType(groupedByTypeAndPeriod, CategoryType.Gain);
        var expenses = FilterByType(groupedByTypeAndPeriod, CategoryType.Expense);

        var currentGains = GetPeriodTotal(gains, dateInterval.Start);
        var currentExpenses = GetPeriodTotal(expenses, dateInterval.Start);

        var averageExpenses = CalculateAverage(expenses);

        return new SummaryResult(
            CurrentExpenses: currentExpenses,
            ExpenseVariation: currentExpenses - averageExpenses,
            NetFlow: currentGains - currentExpenses,
            CurrentGains: currentGains,
            AverageExpenses: averageExpenses);
    }

    private static IEnumerable<(CategoryType Type, (DateOnly From, DateOnly To) Period, Money Total)> GroupByTypeAndPeriod(
        IEnumerable<SummaryData> data,
        DateOnly startDate,
        DateIntervalType intervalType,
        int frequency) =>
        data
            .GroupBy(x => (x.CategoryType, intervalType.Interval(startDate, x.Date, frequency)))
            .Select(g => (
                Type: g.Key.CategoryType,
                Period: g.Key.Item2,
                Total: (Money) g.Select(x => x.Total.Value).Sum()));

    private static Dictionary<(DateOnly From, DateOnly To), Money> FilterByType(
        IEnumerable<(CategoryType Type, (DateOnly From, DateOnly To) Period, Money Total)> data,
        CategoryType categoryType) =>
        data
            .Where(x => x.Type == categoryType)
            .ToDictionary(x => x.Period, x => x.Total);

    private static Money GetPeriodTotal(
        Dictionary<(DateOnly From, DateOnly To), Money> periodData,
        DateOnly asOf) =>
        periodData
            .Where(x => x.Key.From == asOf)
            .Select(x => x.Value)
            .FirstOrDefault(Money.Zero);

    private static Money CalculateAverage(Dictionary<(DateOnly From, DateOnly To), Money> periodData)
    {
        if (periodData.Count == 0)
            return Money.Zero;

        var total = periodData.Values.Select(m => m.Value).Sum();
        return Math.Round(total / periodData.Count, 2);
    }
}
