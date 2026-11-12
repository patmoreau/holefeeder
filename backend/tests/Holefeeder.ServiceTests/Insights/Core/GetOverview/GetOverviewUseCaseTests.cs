using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Insights.Core.GetOverview;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.Insights.TestKit;

namespace Holefeeder.ServiceTests.Insights.Core.GetOverview;

public class GetOverviewUseCaseTests
{
    private static readonly DateOnly Today = new(2026, 5, 15);

    private readonly InsightsInMemory _insights = new();
    private readonly GetOverviewUseCase _useCase;

    public GetOverviewUseCaseTests()
    {
        _useCase = new GetOverviewUseCase(_insights);
        _insights.Use(PeriodSettings.Valid(new DateOnly(2026, 1, 1), IntervalType.Monthly, 1));
    }

    [Fact]
    public async Task HandleAsync_ReportsWhatWasSavedThisPeriod()
    {
        Earn(new DateOnly(2026, 5, 1), 3000m);
        Spend(new DateOnly(2026, 5, 2), 2222.22m);
        Spend(new DateOnly(2026, 4, 30), 999m);

        var result = await OverviewAsync();

        result.Value.Should().BeEquivalentTo(new
        {
            Period = new Period(new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31)),
            Income = 3000m,
            Spending = 2222.22m,
            Net = 777.78m,
            Outcome = NetOutcome.Saved,
            SavedPercentage = 26
        });
    }

    [Fact]
    public async Task HandleAsync_ReportsWhatWasOverspentThisPeriod()
    {
        Spend(new DateOnly(2026, 5, 2), 50m);

        var result = await OverviewAsync();

        result.Value.Should().BeEquivalentTo(new { Net = -50m, Outcome = NetOutcome.Overspent, SavedPercentage = 0 });
    }

    [Fact]
    public async Task HandleAsync_ReportsBreakingEven_WhenIncomeMatchesSpending()
    {
        Earn(new DateOnly(2026, 5, 1), 80m);
        Spend(new DateOnly(2026, 5, 2), 80m);

        var result = await OverviewAsync();

        result.Value.Outcome.Should().Be(NetOutcome.BrokeEven);
    }

    [Fact]
    public async Task HandleAsync_ReportsNoOutcome_WhenThePeriodHasNoActivity()
    {
        var result = await OverviewAsync();

        result.Value.Outcome.Should().Be(NetOutcome.None);
    }

    [Fact]
    public async Task HandleAsync_RoundsTheSavedPercentageToAWholeNumber()
    {
        Earn(new DateOnly(2026, 5, 1), 1000m);
        Spend(new DateOnly(2026, 5, 2), 999.99m);

        var result = await OverviewAsync();

        result.Value.Should().BeEquivalentTo(new { Outcome = NetOutcome.Saved, SavedPercentage = 0 });
    }

    [Fact]
    public async Task HandleAsync_ListsTheLastPeriodsOldestFirst_WithEmptyPeriodsAtZero()
    {
        Earn(new DateOnly(2026, 3, 10), 100m);
        Spend(new DateOnly(2026, 3, 31), 40m);
        Spend(new DateOnly(2026, 5, 1), 60m);
        Earn(new DateOnly(2026, 2, 28), 999m);
        Earn(new DateOnly(2026, 6, 1), 999m);

        var result = await OverviewAsync(3);

        result.Value.Series.Should().Equal(
            new PeriodFlow(new Period(new DateOnly(2026, 3, 1), new DateOnly(2026, 3, 31)), 100m, 40m, 60m),
            new PeriodFlow(new Period(new DateOnly(2026, 4, 1), new DateOnly(2026, 4, 30)), 0m, 0m, 0m),
            new PeriodFlow(new Period(new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31)), 0m, 60m, -60m));
        result.Value.Should().BeEquivalentTo(new { TotalIncome = 100m, TotalSpending = 100m, TotalNet = 0m });
    }

    [Fact]
    public async Task HandleAsync_DescribesTheRangeInTheBudgetsUnit()
    {
        var result = await OverviewAsync(12);

        result.Value.Range.Should().Be(new RangeSpan(RangeUnit.Year, 1));
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheFailure_WhenInsightsCannotBeRead()
    {
        var failure = new ResultError("insights-unavailable", "Insights are unavailable.");
        _insights.FailWith(failure);

        var result = await OverviewAsync();

        result.Should().BeFailure().And.WithError(failure);
    }

    private void Earn(DateOnly date, decimal amount) => _insights.Earn(_insights.Add(CategoryType.Gain, $"gain-{date}"), date, amount);

    private void Spend(DateOnly date, decimal amount) => _insights.Spend(_insights.Add(CategoryType.Expense, $"expense-{date}"), date, amount);

    private Task<Result<Overview>> OverviewAsync(int periods = 6) =>
        _useCase.HandleAsync(new GetOverviewRequest(AUser(), Today, PeriodCount.Valid(periods)), CancellationToken.None);
}
