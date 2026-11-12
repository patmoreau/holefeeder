using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

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
        var expenses = TotalsByPeriod(totalsResult.Value, CategoryType.Expense, current, settingsResult.Value);
        var gains = TotalsByPeriod(totalsResult.Value, CategoryType.Gain, current, settingsResult.Value);

        var currentExpenses = expenses.GetValueOrDefault(current);
        var currentGains = gains.GetValueOrDefault(current);
        var averageExpenses = PeriodTotals.AverageOf(expenses);
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

    private static Dictionary<Period, decimal> TotalsByPeriod(
        IEnumerable<DailyTotal> totals, CategoryType categoryType, Period current, PeriodSettings settings) =>
        PeriodTotals.ByPeriod(
            totals.Where(total => total.CategoryType == categoryType).Select(total => (total.Date, total.Total)),
            current,
            settings);
}
