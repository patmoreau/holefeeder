using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Insights.Core;
using Holefeeder.Service.Insights.Core.GetSpending;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.Insights.TestKit;

namespace Holefeeder.ServiceTests.Insights.Core.GetSpending;

public class GetSpendingUseCaseTests
{
    private static readonly DateOnly Today = new(2026, 5, 15);

    private readonly InsightsInMemory _insights = new();
    private readonly GetSpendingUseCase _useCase;
    private readonly InsightCategory _groceries;
    private readonly InsightCategory _dining;
    private readonly InsightCategory _unused;

    public GetSpendingUseCaseTests()
    {
        _useCase = new GetSpendingUseCase(_insights);
        _insights.Use(PeriodSettings.Valid(new DateOnly(2026, 1, 1), IntervalType.Monthly, 1));
        _groceries = _insights.Add(CategoryType.Expense, "Groceries", budget: 500m, color: "#34C759");
        _dining = _insights.Add(CategoryType.Expense, "Dining", color: "#FF2D55");
        _unused = _insights.Add(CategoryType.Expense, "Unused");
        _insights.Add(CategoryType.Gain, "Salary");

        _insights.Spend(_groceries, new DateOnly(2026, 2, 10), 100m);
        _insights.Spend(_groceries, new DateOnly(2026, 3, 5), 200m, "costco");
        _insights.Spend(_groceries, new DateOnly(2026, 3, 20), 30m, "costco,weekly");
        _insights.Spend(_groceries, new DateOnly(2026, 4, 10), 300m, "weekly");
        _insights.Spend(_groceries, new DateOnly(2026, 5, 1), 150m, "costco, weekly");
        _insights.Spend(_groceries, new DateOnly(2026, 5, 31), 50m, "weekly");
        _insights.Spend(_dining, new DateOnly(2026, 1, 15), 90m, "weekly");
        _insights.Spend(_dining, new DateOnly(2026, 5, 20), 60m, " ,");
        _insights.Spend(_dining, new DateOnly(2026, 6, 1), 999m, "weekly");
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheCurrentPeriod()
    {
        var result = await SpendingAsync();

        result.Value.Period.Should().Be(new Period(new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31)));
    }

    [Fact]
    public async Task HandleAsync_ListsEachCategoryWithItsSpendingAndItsAverageSinceItsFirstPeriod()
    {
        var result = await SpendingAsync();

        result.Value.Categories.Select(category => (category.Name, category.Budget, category.Amount, category.Average)).Should().Equal(
            ("Groceries", 500m, 200m, 210m),
            ("Dining", 0m, 60m, 22.5m),
            ("Unused", 0m, 0m, 0m));
        result.Value.Categories[0].Should().BeEquivalentTo(new { _groceries.Id, Color = "#34C759" });
    }

    [Fact]
    public async Task HandleAsync_TellsHowMuchOfItsBudgetEachCategoryUsed()
    {
        var rent = _insights.Add(CategoryType.Expense, "Rent", budget: 50m);
        _insights.Spend(rent, new DateOnly(2026, 5, 3), 60m);

        var result = await SpendingAsync();

        result.Value.Categories.Select(category => (category.Name, category.BudgetStatus, category.BudgetPercentage)).Should().Equal(
            ("Groceries", BudgetStatus.Under, 40),
            ("Dining", BudgetStatus.None, 0),
            ("Rent", BudgetStatus.Over, 120),
            ("Unused", BudgetStatus.None, 0));
    }

    [Fact]
    public async Task HandleAsync_IsUnderBudget_WhenSpendingMatchesTheBudget()
    {
        var phone = _insights.Add(CategoryType.Expense, "Phone", budget: 75.5m);
        _insights.Spend(phone, new DateOnly(2026, 5, 3), 75.5m);

        var result = await SpendingAsync();

        result.Value.Categories.Single(category => category.Name == "Phone")
            .Should().BeEquivalentTo(new { BudgetStatus = BudgetStatus.Under, BudgetPercentage = 100 });
    }

    [Fact]
    public async Task HandleAsync_OrdersCategoriesByAverageThenSpendingThenName()
    {
        var zebra = _insights.Add(CategoryType.Expense, "Zebra");
        var apple = _insights.Add(CategoryType.Expense, "Apple");
        _insights.Spend(zebra, new DateOnly(2026, 5, 3), 60m);
        _insights.Spend(apple, new DateOnly(2026, 5, 4), 60m);

        var result = await SpendingAsync();

        result.Value.Categories.Select(category => category.Name).Should().Equal("Groceries", "Dining", "Apple", "Zebra", "Unused");
    }

    [Fact]
    public async Task HandleAsync_RoundsTheAverageToCents()
    {
        var gym = _insights.Add(CategoryType.Expense, "Gym");
        _insights.Spend(gym, new DateOnly(2026, 2, 1), 100m);

        var result = await SpendingAsync();

        result.Value.Categories.Single(category => category.Name == "Gym").Average.Should().Be(33.33m);
    }

    [Fact]
    public async Task HandleAsync_ListsEachTagWithItsSpendingAndItsAverageSinceItsFirstPeriod()
    {
        var result = await SpendingAsync();

        result.Value.Tags.Should().Equal(new TagBreakdown("costco", 150m, 115m), new TagBreakdown("weekly", 200m, 105m));
    }

    [Fact]
    public async Task HandleAsync_ListsTheTagsOfEachCategoryThisPeriod()
    {
        var result = await SpendingAsync();

        result.Value.Categories[0].Tags.Should().Equal(new TagAmount("weekly", 200m), new TagAmount("costco", 150m));
        result.Value.Categories[1].Tags.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_ComparesThePeriodWithTheAverageOfEveryPeriodWithSpending()
    {
        var result = await SpendingAsync();

        result.Value.Should().BeEquivalentTo(new { Current = 260m, Average = 329.83m, Variation = -69.83m, VariationPercentage = -21.17m });
    }

    [Fact]
    public async Task HandleAsync_HasNoPreviousAverages_WhenTheBudgetIsOneTime()
    {
        _insights.Use(PeriodSettings.Valid(new DateOnly(2026, 1, 1), IntervalType.OneTime, 1));

        var result = await SpendingAsync();

        result.Value.Categories.Should().OnlyContain(category => category.Average == 0m);
        result.Value.Tags.Should().OnlyContain(tag => tag.Average == 0m);
    }

    [Fact]
    public async Task HandleAsync_HasNoVariationPercentage_WhenNothingWasEverSpent()
    {
        var empty = new InsightsInMemory();
        empty.Use(PeriodSettings.Valid(new DateOnly(2026, 1, 1), IntervalType.Monthly, 1));

        var result = await new GetSpendingUseCase(empty).HandleAsync(new GetSpendingRequest(AUser(), Today), CancellationToken.None);

        result.Value.Should().BeEquivalentTo(new { Current = 0m, Average = 0m, Variation = 0m, VariationPercentage = 0m, Tags = Array.Empty<TagBreakdown>() });
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheFailure_WhenInsightsCannotBeRead()
    {
        var failure = new ResultError("insights-unavailable", "Insights are unavailable.");
        _insights.FailWith(failure);

        var result = await SpendingAsync();

        result.Should().BeFailure().And.WithError(failure);
    }

    private Task<Result<Breakdown>> SpendingAsync() =>
        _useCase.HandleAsync(new GetSpendingRequest(AUser(), Today), CancellationToken.None);
}
