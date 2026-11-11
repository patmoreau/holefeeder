using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Dashboard.Core;
using Holefeeder.Service.Dashboard.Core.GetUpcomingFlows;
using Holefeeder.ServiceTests.Dashboard.TestKit;

namespace Holefeeder.ServiceTests.Dashboard.Core.GetUpcomingFlows;

public class GetUpcomingFlowsUseCaseTests
{
    private static readonly DateOnly Today = new(2026, 11, 10);

    private readonly DashboardInMemory _dashboard = new();
    private readonly GetUpcomingFlowsUseCase _useCase;

    public GetUpcomingFlowsUseCaseTests() => _useCase = new GetUpcomingFlowsUseCase(_dashboard);

    [Fact]
    public async Task HandleAsync_ListsEachUnpaidOccurrenceUntilTheEndOfThePeriod()
    {
        var user = AMonthlyUser();
        var rent = ACashflow(new DateOnly(2026, 11, 15), IntervalType.Monthly, 1, 1450m, "Rent", "Housing", "#007AFF", ["housing"]);
        _dashboard.Add(user, rent);

        var result = await _useCase.HandleAsync(new GetUpcomingFlowsRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().ContainSingle().Which.Should().BeEquivalentTo(
            new DashboardUpcomingFlow(rent.Id, new DateOnly(2026, 11, 15), "Rent", "Housing", "#007AFF", ["housing"], 1450m, CategoryType.Expense));
    }

    [Fact]
    public async Task HandleAsync_TellsWhetherTheFlowIsAGainOrAnExpense()
    {
        var user = AMonthlyUser();
        _dashboard.Add(user, ACashflow(new DateOnly(2026, 11, 15), IntervalType.OneTime, 1, 1000m, "Salary", "Pay check", "#34C759", [], categoryType: CategoryType.Gain));

        var result = await _useCase.HandleAsync(new GetUpcomingFlowsRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().ContainSingle().Which.CategoryType.Should().Be(CategoryType.Gain);
    }

    [Fact]
    public async Task HandleAsync_ListsEveryRepeatWithinThePeriod()
    {
        var user = AMonthlyUser();
        _dashboard.Add(user, ACashflow(new DateOnly(2026, 11, 13), IntervalType.Weekly, 1, 50m, "Groceries", "Food", "#FF8D28", []));

        var result = await _useCase.HandleAsync(new GetUpcomingFlowsRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Select(flow => flow.Date).Should().Equal(
            new DateOnly(2026, 11, 13), new DateOnly(2026, 11, 20), new DateOnly(2026, 11, 27));
    }

    [Fact]
    public async Task HandleAsync_KeepsOverdueOccurrencesAndSkipsPaidOnes()
    {
        var user = AMonthlyUser();
        _dashboard.Add(user, ACashflow(
            new DateOnly(2026, 9, 5), IntervalType.Monthly, 1, 88.4m, "Hydro", "Utilities", "#34C759", [],
            lastPaidDate: new DateOnly(2026, 9, 6), lastCashflowDate: new DateOnly(2026, 9, 5)));

        var result = await _useCase.HandleAsync(new GetUpcomingFlowsRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Select(flow => flow.Date).Should().Equal(new DateOnly(2026, 10, 5), new DateOnly(2026, 11, 5));
    }

    [Fact]
    public async Task HandleAsync_LeavesOutOccurrencesAfterThePeriod()
    {
        var user = AMonthlyUser();
        _dashboard.Add(user, ACashflow(new DateOnly(2026, 12, 1), IntervalType.Monthly, 1, 1450m, "Rent", "Housing", "#007AFF", []));

        var result = await _useCase.HandleAsync(new GetUpcomingFlowsRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_SortsByDateThenTitle()
    {
        var user = AMonthlyUser();
        _dashboard.Add(user, ACashflow(new DateOnly(2026, 11, 20), IntervalType.OneTime, 1, 1m, "Water", "Utilities", "#000000", []));
        _dashboard.Add(user, ACashflow(new DateOnly(2026, 11, 12), IntervalType.OneTime, 1, 1m, "Phone", "Utilities", "#000000", []));
        _dashboard.Add(user, ACashflow(new DateOnly(2026, 11, 20), IntervalType.OneTime, 1, 1m, "Internet", "Utilities", "#000000", []));

        var result = await _useCase.HandleAsync(new GetUpcomingFlowsRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Select(flow => flow.Title).Should().Equal("Phone", "Internet", "Water");
    }

    [Fact]
    public async Task HandleAsync_TitlesAFlowWithoutDescriptionWithItsCategory()
    {
        var user = AMonthlyUser();
        _dashboard.Add(user, ACashflow(new DateOnly(2026, 11, 15), IntervalType.OneTime, 1, 1000m, " ", "Pay check", "#34C759", []));

        var result = await _useCase.HandleAsync(new GetUpcomingFlowsRequest(user, Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().ContainSingle().Which.Should().BeEquivalentTo(new { Title = "Pay check", Category = (string?)null });
    }

    [Fact]
    public async Task HandleAsync_IgnoresOtherUsersCashflows()
    {
        _dashboard.Add(AMonthlyUser(), ACashflow(new DateOnly(2026, 11, 15), IntervalType.OneTime, 1, 1m, "Other", "Food", "#000000", []));

        var result = await _useCase.HandleAsync(new GetUpcomingFlowsRequest(AMonthlyUser(), Today), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_ReturnsDashboardError_WhenDashboardFails()
    {
        var error = new ResultError("dashboard-failed", "Dashboard failed.");
        _dashboard.FailWith(error);

        var result = await _useCase.HandleAsync(new GetUpcomingFlowsRequest(AUser(), Today), CancellationToken.None);

        result.Should().BeFailure().And.WithError(error);
    }

    private Holefeeder.Service.Support.Core.User AMonthlyUser()
    {
        var user = AUser();
        _dashboard.Add(user, PeriodSettings.Valid(new DateOnly(2025, 1, 1), IntervalType.Monthly, 1));
        return user;
    }

    private static UpcomingCashflow ACashflow(
        DateOnly effectiveDate, IntervalType intervalType, int frequency, decimal amount,
        string description, string categoryName, string categoryColor, string[] tags,
        DateOnly? lastPaidDate = null, DateOnly? lastCashflowDate = null, CategoryType categoryType = CategoryType.Expense) =>
        new(
            AnId(),
            new Cashflow(AnId(), effectiveDate, intervalType, frequency, amount, categoryType, lastPaidDate, lastCashflowDate),
            description,
            categoryName,
            categoryColor,
            tags);
}
