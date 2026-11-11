using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Dashboard.Core;
using Holefeeder.Service.Dashboard.Core.GetDashboardSummary;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.Dashboard.TestKit;

namespace Holefeeder.ServiceTests.Dashboard.Core.GetDashboardSummary;

public class GetDashboardSummaryUseCaseTests
{
    private static readonly DateOnly Today = new(2026, 11, 10);

    private readonly DashboardInMemory _dashboard = new();
    private readonly GetDashboardSummaryUseCase _useCase;

    public GetDashboardSummaryUseCaseTests() => _useCase = new GetDashboardSummaryUseCase(_dashboard);

    [Fact]
    public async Task HandleAsync_ComparesCurrentPeriodWithTheAverageOfAllPeriods()
    {
        var user = AUser();
        _dashboard.Add(user, PeriodSettings.Valid(new DateOnly(2025, 1, 1), IntervalType.Monthly, 1));
        _dashboard.Add(user, new DailyTotal(CategoryType.Expense, new DateOnly(2026, 9, 1), 100.1m));
        _dashboard.Add(user, new DailyTotal(CategoryType.Expense, new DateOnly(2026, 10, 1), 200.2m));
        _dashboard.Add(user, new DailyTotal(CategoryType.Expense, new DateOnly(2026, 11, 1), 800.8m));
        _dashboard.Add(user, new DailyTotal(CategoryType.Expense, new DateOnly(2026, 11, 10), 400.4m));
        _dashboard.Add(user, new DailyTotal(CategoryType.Gain, new DateOnly(2026, 9, 1), 1000.1m));
        _dashboard.Add(user, new DailyTotal(CategoryType.Gain, new DateOnly(2026, 10, 1), 2000.2m));
        _dashboard.Add(user, new DailyTotal(CategoryType.Gain, new DateOnly(2026, 11, 1), 3000.3m));

        var result = await _useCase.HandleAsync(new GetDashboardSummaryRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful()
            .And.WithValue(new DashboardSummary(
                CurrentExpenses: 1201.2m,
                ExpenseVariation: 700.7m,
                ExpenseVariationPercentage: 140m,
                NetFlow: 1799.1m,
                CurrentGains: 3000.3m,
                AverageExpenses: 500.5m,
                ProjectedNetFlow: 1799.1m));
    }

    [Fact]
    public async Task HandleAsync_ReturnsZeros_WhenThereAreNoTransactions()
    {
        var result = await _useCase.HandleAsync(new GetDashboardSummaryRequest(AUser(), Today), CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(new DashboardSummary(0, 0, 0, 0, 0, 0, 0));
    }

    [Fact]
    public async Task HandleAsync_ReportsOnlyGains_WhenThereAreNoExpenses()
    {
        var user = AUser();
        _dashboard.Add(user, new DailyTotal(CategoryType.Gain, Today, 1500m));

        var result = await _useCase.HandleAsync(new GetDashboardSummaryRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(new DashboardSummary(0, 0, 0, 1500m, 1500m, 0, 1500m));
    }

    [Fact]
    public async Task HandleAsync_LeavesOtherPeriodsOutOfCurrentTotals_ButAveragesThem()
    {
        var user = AUser();
        _dashboard.Add(user, PeriodSettings.Valid(new DateOnly(2025, 1, 1), IntervalType.Monthly, 1));
        _dashboard.Add(user, new DailyTotal(CategoryType.Expense, new DateOnly(2026, 10, 31), 300m));
        _dashboard.Add(user, new DailyTotal(CategoryType.Expense, new DateOnly(2026, 11, 30), 100m));
        _dashboard.Add(user, new DailyTotal(CategoryType.Expense, new DateOnly(2026, 12, 1), 200m));
        _dashboard.Add(user, new DailyTotal(CategoryType.Gain, new DateOnly(2026, 12, 1), 900m));

        var result = await _useCase.HandleAsync(new GetDashboardSummaryRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful()
            .And.WithValue(new DashboardSummary(100m, -100m, -50m, -100m, 0, 200m, -100m));
    }

    [Fact]
    public async Task HandleAsync_UsesThePeriodOfTheSettings_WhenWeeklyEveryTwoWeeks()
    {
        var user = AUser();
        _dashboard.Add(user, PeriodSettings.Valid(new DateOnly(2026, 10, 29), IntervalType.Weekly, 2));
        _dashboard.Add(user, new DailyTotal(CategoryType.Expense, new DateOnly(2026, 10, 28), 40m));
        _dashboard.Add(user, new DailyTotal(CategoryType.Expense, new DateOnly(2026, 10, 29), 10m));
        _dashboard.Add(user, new DailyTotal(CategoryType.Expense, new DateOnly(2026, 11, 11), 20m));
        _dashboard.Add(user, new DailyTotal(CategoryType.Expense, new DateOnly(2026, 11, 12), 80m));

        var result = await _useCase.HandleAsync(new GetDashboardSummaryRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful()
            .And.WithValue(new DashboardSummary(30m, -20m, -40m, -30m, 0, 50m, -30m));
    }

    [Fact]
    public async Task HandleAsync_RoundsTheAverageToCents()
    {
        var user = AUser();
        _dashboard.Add(user, PeriodSettings.Valid(new DateOnly(2025, 1, 1), IntervalType.Monthly, 1));
        _dashboard.Add(user, new DailyTotal(CategoryType.Expense, new DateOnly(2026, 9, 1), 10m));
        _dashboard.Add(user, new DailyTotal(CategoryType.Expense, new DateOnly(2026, 10, 1), 10m));
        _dashboard.Add(user, new DailyTotal(CategoryType.Expense, new DateOnly(2026, 11, 1), 10.01m));

        var result = await _useCase.HandleAsync(new GetDashboardSummaryRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful()
            .And.WithValue(new DashboardSummary(10.01m, 0.01m, 0.1m, -10.01m, 0, 10m, -10.01m));
    }

    [Fact]
    public async Task HandleAsync_IgnoresOtherUsersTotals()
    {
        var user = AUser();
        _dashboard.Add(AUser(), new DailyTotal(CategoryType.Expense, Today, 75m));

        var result = await _useCase.HandleAsync(new GetDashboardSummaryRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(new DashboardSummary(0, 0, 0, 0, 0, 0, 0));
    }

    [Fact]
    public async Task HandleAsync_ProjectsNetFlowWithUnpaidCashflowsUntilTheEndOfThePeriod()
    {
        var user = AUser();
        _dashboard.Add(user, PeriodSettings.Valid(new DateOnly(2025, 1, 1), IntervalType.Monthly, 1));
        _dashboard.Add(user, new DailyTotal(CategoryType.Expense, new DateOnly(2026, 11, 2), 150m));
        _dashboard.Add(user, new DailyTotal(CategoryType.Gain, new DateOnly(2026, 11, 1), 1000m));
        _dashboard.Add(user, new Cashflow(
            AnId(), new DateOnly(2026, 1, 15), IntervalType.Monthly, 1, 1450m, CategoryType.Expense,
            LastPaidDate: new DateOnly(2026, 10, 14), LastCashflowDate: new DateOnly(2026, 10, 15)));
        _dashboard.Add(user, new Cashflow(
            AnId(), new DateOnly(2026, 11, 6), IntervalType.Weekly, 1, 25m, CategoryType.Expense, null, null));
        _dashboard.Add(user, new Cashflow(
            AnId(), new DateOnly(2026, 11, 13), IntervalType.Weekly, 2, 1000m, CategoryType.Gain, null, null));
        _dashboard.Add(user, new Cashflow(
            AnId(), new DateOnly(2026, 12, 1), IntervalType.Monthly, 1, 500m, CategoryType.Expense, null, null));

        var result = await _useCase.HandleAsync(new GetDashboardSummaryRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.NetFlow.Should().Be(850m);
        result.Value.ProjectedNetFlow.Should().Be(850m - 1450m - 4 * 25m + 2 * 1000m);
    }

    [Fact]
    public async Task HandleAsync_IgnoresOtherUsersCashflows()
    {
        var user = AUser();
        _dashboard.Add(AUser(), new Cashflow(AnId(), Today, IntervalType.OneTime, 1, 300m, CategoryType.Expense, null, null));

        var result = await _useCase.HandleAsync(new GetDashboardSummaryRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(new DashboardSummary(0, 0, 0, 0, 0, 0, 0));
    }

    [Fact]
    public async Task HandleAsync_ReturnsDashboardError_WhenDashboardFails()
    {
        var error = new ResultError("dashboard-failed", "Dashboard failed.");
        _dashboard.FailWith(error);

        var result = await _useCase.HandleAsync(new GetDashboardSummaryRequest(AUser(), Today), CancellationToken.None);

        result.Should().BeFailure().And.WithError(error);
    }
}
