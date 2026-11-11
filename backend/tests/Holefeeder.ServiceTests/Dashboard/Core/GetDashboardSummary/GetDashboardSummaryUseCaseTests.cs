using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Dashboard.Core;
using Holefeeder.Service.Dashboard.Core.GetDashboardSummary;
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
                AverageExpenses: 500.5m));
    }

    [Fact]
    public async Task HandleAsync_ReturnsZeros_WhenThereAreNoTransactions()
    {
        var result = await _useCase.HandleAsync(new GetDashboardSummaryRequest(AUser(), Today), CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(new DashboardSummary(0, 0, 0, 0, 0, 0));
    }

    [Fact]
    public async Task HandleAsync_ReportsOnlyGains_WhenThereAreNoExpenses()
    {
        var user = AUser();
        _dashboard.Add(user, new DailyTotal(CategoryType.Gain, Today, 1500m));

        var result = await _useCase.HandleAsync(new GetDashboardSummaryRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(new DashboardSummary(0, 0, 0, 1500m, 1500m, 0));
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
            .And.WithValue(new DashboardSummary(100m, -100m, -50m, -100m, 0, 200m));
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
            .And.WithValue(new DashboardSummary(30m, -20m, -40m, -30m, 0, 50m));
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
            .And.WithValue(new DashboardSummary(10.01m, 0.01m, 0.1m, -10.01m, 0, 10m));
    }

    [Fact]
    public async Task HandleAsync_IgnoresOtherUsersTotals()
    {
        var user = AUser();
        _dashboard.Add(AUser(), new DailyTotal(CategoryType.Expense, Today, 75m));

        var result = await _useCase.HandleAsync(new GetDashboardSummaryRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(new DashboardSummary(0, 0, 0, 0, 0, 0));
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
