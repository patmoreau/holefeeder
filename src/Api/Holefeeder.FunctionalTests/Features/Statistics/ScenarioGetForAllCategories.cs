using Holefeeder.Application.Features.Statistics.Queries;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.FunctionalTests.StepDefinitions;
using Holefeeder.Tests.Common.Builders.Accounts;
using Holefeeder.Tests.Common.Builders.Categories;
using Holefeeder.Tests.Common.Builders.Transactions;

namespace Holefeeder.FunctionalTests.Features.Statistics;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetForAllCategories(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper) : HolefeederScenario(applicationDriver, testOutputHelper)
{
    private readonly Guid _userId = UserStepDefinition.HolefeederUserId;

    private readonly Dictionary<string, Category> _categories = new();
    private readonly Dictionary<string, Account> _accounts = new();

    [Fact]
    public Task WhenGettingStatistics() =>
        ScenarioFor("A user sends was to know his statistics", runner =>
            runner.Given("the user is authorized", GivenUserIsAuthorized)
                .And("a 'purchase' transaction was made on the 'credit card' account in February 2023", () => CreateTransaction("purchase", "credit card", new DateOnly(2023, 2, 1), 500.5m))
                .And("a 'food and drink' transaction was made on the 'credit card' account in December 2022", () => CreateTransaction("food and drink", "credit card", new DateOnly(2022, 12, 1), 100.1m))
                .And("a 'food and drink' transaction was made on the 'credit card' account in January 2023", () => CreateTransaction("food and drink", "credit card", new DateOnly(2023, 1, 1), 200.2m))
                .And("a 'food and drink' transaction was made on the 'credit card' account in February 2023", () => CreateTransaction("food and drink", "credit card", new DateOnly(2023, 2, 1), 300.3m))
                .And("a second 'food and drink' transaction was made on the 'checking' account in February 2023", () => CreateTransaction("food and drink", "checking", new DateOnly(2023, 2, 10), 400.4m))
                .When("user gets their statistics", () => QueryEndpoint(ApiResources.GetForAllCategories))
                .Then("the total for the year should match the expected", ValidateResponse));

    private Task ValidateResponse()
    {
        var expectedFoodAndDrink = new StatisticsDto(_categories["food and drink"].Id,
            _categories["food and drink"].Name, _categories["food and drink"].Color, 333.67m,
            new[]
            {
                new YearStatisticsDto(2022, 100.1m, new[] {new MonthStatisticsDto(12, 100.1m)}), new YearStatisticsDto(2023, 900.9m,
                    new[] {new MonthStatisticsDto(1, 200.2m), new MonthStatisticsDto(2, 700.7m)})
            });
        var expectedPurchase = new StatisticsDto(_categories["purchase"].Id,
            _categories["purchase"].Name, _categories["purchase"].Color, 500.5m,
            new[] { new YearStatisticsDto(2023, 500.5m, new[] { new MonthStatisticsDto(2, 500.5m) }) });

        var results = HttpClientDriver.DeserializeContent<StatisticsDto[]>();
        if (results is null)
        {
            results.Should()
                .NotBeNull();
        }

        results.Should()
            .NotBeNull()
            .And.HaveCount(2)
            .And.SatisfyRespectively(
                first => first.Should().BeEquivalentTo(expectedFoodAndDrink),
                second => second.Should().BeEquivalentTo(expectedPurchase)
            );

        return Task.CompletedTask;
    }

    private async Task CreateTransaction(string categoryName, string accountName, DateOnly date, decimal amount)
    {
        if (!_categories.TryGetValue(categoryName, out var category))
        {
            category = await CategoryBuilder.GivenACategory().OfType(CategoryType.Expense).WithName(categoryName).ForUser(_userId).SavedInDbAsync(DatabaseDriver);
            _categories.Add(categoryName, category);
        }

        if (!_accounts.TryGetValue(accountName, out var account))
        {
            account = await AccountBuilder.GivenAnActiveAccount().WithName(accountName).ForUser(_userId).SavedInDbAsync(DatabaseDriver);
            _accounts.Add(accountName, account);
        }

        await TransactionBuilder
            .GivenATransaction()
            .ForAccount(account)
            .ForCategory(category)
            .OnDate(date)
            .OfAmount(amount)
            .SavedInDbAsync(DatabaseDriver);
    }
}
