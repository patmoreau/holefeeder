using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Insights.Core;
using Holefeeder.Service.Insights.Core.GetIncome;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.Insights.TestKit;

namespace Holefeeder.ServiceTests.Insights.Core.GetIncome;

public class GetIncomeUseCaseTests
{
    private static readonly DateOnly Today = new(2026, 5, 15);

    private readonly InsightsInMemory _insights = new();
    private readonly GetIncomeUseCase _useCase;

    public GetIncomeUseCaseTests()
    {
        _useCase = new GetIncomeUseCase(_insights);
        _insights.Use(PeriodSettings.Valid(new DateOnly(2026, 1, 1), IntervalType.Monthly, 1));
        var salary = _insights.Add(CategoryType.Gain, "Salary");
        var groceries = _insights.Add(CategoryType.Expense, "Groceries");
        _insights.Earn(salary, new DateOnly(2026, 4, 1), 3000m, "job");
        _insights.Earn(salary, new DateOnly(2026, 5, 1), 3500m, "job");
        _insights.Spend(groceries, new DateOnly(2026, 5, 2), 200m, "weekly");
    }

    [Fact]
    public async Task HandleAsync_BreaksDownTheIncomeOnly()
    {
        var result = await IncomeAsync();

        result.Value.Should().BeEquivalentTo(new { Current = 3500m, Average = 3250m, Variation = 250m, VariationPercentage = 7.69m });
        result.Value.Categories.Select(category => (category.Name, category.Amount, category.Average)).Should().Equal(("Salary", 3500m, 3000m));
        result.Value.Tags.Should().Equal(new TagBreakdown("job", 3500m, 3000m));
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheFailure_WhenInsightsCannotBeRead()
    {
        var failure = new ResultError("insights-unavailable", "Insights are unavailable.");
        _insights.FailWith(failure);

        var result = await IncomeAsync();

        result.Should().BeFailure().And.WithError(failure);
    }

    private Task<Result<Breakdown>> IncomeAsync() =>
        _useCase.HandleAsync(new GetIncomeRequest(AUser(), Today), CancellationToken.None);
}
