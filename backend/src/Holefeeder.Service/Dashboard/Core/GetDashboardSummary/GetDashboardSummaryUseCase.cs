using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Dashboard.Core.GetDashboardSummary;

internal sealed class GetDashboardSummaryUseCase(IDashboard dashboard)
{
    public async Task<Result<DashboardSummary>> HandleAsync(GetDashboardSummaryRequest request, CancellationToken cancellationToken)
    {
        var settingsResult = await dashboard.GetPeriodSettingsAsync(request.User, request.Today, cancellationToken);
        if (settingsResult.IsFailure)
        {
            return settingsResult.Error;
        }

        var totalsResult = await dashboard.GetDailyTotalsAsync(request.User, cancellationToken);
        if (totalsResult.IsFailure)
        {
            return totalsResult.Error;
        }

        var cashflowsResult = await dashboard.GetActiveCashflowsAsync(request.User, cancellationToken);
        if (cashflowsResult.IsFailure)
        {
            return cashflowsResult.Error;
        }

        var current = Period.Containing(request.Today, settingsResult.Value);
        var periods = PeriodsAnchoredOn(current, settingsResult.Value);
        var expenses = TotalsByPeriod(totalsResult.Value, CategoryType.Expense, periods);
        var gains = TotalsByPeriod(totalsResult.Value, CategoryType.Gain, periods);

        var currentExpenses = expenses.GetValueOrDefault(current);
        var currentGains = gains.GetValueOrDefault(current);
        var averageExpenses = expenses.Count == 0 ? 0 : Math.Round(expenses.Values.Sum() / expenses.Count, 2);
        var expenseVariation = currentExpenses - averageExpenses;
        var expenseVariationPercentage = averageExpenses == 0 ? 0 : Math.Round(expenseVariation / averageExpenses * 100, 2);
        var netFlow = currentGains - currentExpenses;
        var projectedNetFlow = netFlow + cashflowsResult.Value.Sum(cashflow => cashflow.UpcomingVariation(current.End));

        return new DashboardSummary(
            currentExpenses,
            expenseVariation,
            expenseVariationPercentage,
            netFlow,
            currentGains,
            averageExpenses,
            projectedNetFlow);
    }

    private static PeriodSettings PeriodsAnchoredOn(Period current, PeriodSettings settings) =>
        settings.IntervalType == IntervalType.OneTime
            ? PeriodSettings.Valid(current.Start, IntervalType.Daily, 1)
            : PeriodSettings.Valid(current.Start, settings.IntervalType, settings.Frequency);

    private static Dictionary<Period, decimal> TotalsByPeriod(IEnumerable<DailyTotal> totals, CategoryType categoryType, PeriodSettings periods) =>
        totals
            .Where(total => total.CategoryType == categoryType)
            .GroupBy(total => Period.Containing(total.Date, periods))
            .ToDictionary(group => group.Key, group => group.Sum(total => total.Total));
}
