using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Application.Features.Statistics.Queries;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.Tests.Common.Builders.Accounts;
using Holefeeder.Tests.Common.Builders.Categories;
using Holefeeder.Tests.Common.Builders.Transactions;

using Refit;

namespace Holefeeder.FunctionalTests.Features.Statistics;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetForAllCategories(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper) : HolefeederScenario(applicationDriver, testOutputHelper)
{
    private readonly UserId _userId = TestUsers[AuthorizedUser].UserId;

    private readonly Dictionary<string, Category> _categories = new();
    private readonly Dictionary<string, Account> _accounts = new();

    [Fact]
    public Task WhenGettingStatistics() =>
        ScenarioRunner.Create("A user sends was to know his statistics", ScenarioOutput)
            .Given("a 'purchase' transaction was made on the 'credit card' account in February 2023", () => CreateTransaction("purchase", "credit card", new DateOnly(2023, 2, 1), Money.Create(500.5m).Value))
            .And("a 'food and drink' transaction was made on the 'credit card' account in December 2022", () => CreateTransaction("food and drink", "credit card", new DateOnly(2022, 12, 1), Money.Create(100.1m).Value))
            .And("a 'food and drink' transaction was made on the 'credit card' account in January 2023", () => CreateTransaction("food and drink", "credit card", new DateOnly(2023, 1, 1), Money.Create(200.2m).Value))
            .And("a 'food and drink' transaction was made on the 'credit card' account in February 2023", () => CreateTransaction("food and drink", "credit card", new DateOnly(2023, 2, 1), Money.Create(300.3m).Value))
            .And("a second 'food and drink' transaction was made on the 'checking' account in February 2023", () => CreateTransaction("food and drink", "checking", new DateOnly(2023, 2, 10), Money.Create(400.4m).Value))
            .When(TheUser.GetsTheirStatistics)
            .Then(TheTotalForTheYearShouldMatchTheExpected)
            .PlayAsync();

    private void TheTotalForTheYearShouldMatchTheExpected(IStepRunner runner) =>
        runner.Execute<IApiResponse<IEnumerable<StatisticsDto>>>(response =>
        {
            var expectedFoodAndDrink = new StatisticsDto(_categories["food and drink"].Id,
                _categories["food and drink"].Name, _categories["food and drink"].Color, 333.67m,
                [
                    new YearStatisticsDto(2022, 100.1m, new[] {new MonthStatisticsDto(12, 100.1m)}), new YearStatisticsDto(2023, 900.9m,
                        [new MonthStatisticsDto(1, 200.2m), new MonthStatisticsDto(2, 700.7m)])
                ]);
            var expectedPurchase = new StatisticsDto(_categories["purchase"].Id,
                _categories["purchase"].Name, _categories["purchase"].Color, 500.5m,
                new[] {new YearStatisticsDto(2023, 500.5m, new[] {new MonthStatisticsDto(2, 500.5m)})});

            response.Should().BeValid()
                .And.Subject.Value.Should().BeSuccessful()
                .And.HaveContent();
            var results = response.Value.Content;
            results.Should()
                .NotBeNull()
                .And.HaveCount(2)
                .And.SatisfyRespectively(
                    first => first.Should().BeEquivalentTo(expectedFoodAndDrink),
                    second => second.Should().BeEquivalentTo(expectedPurchase)
                );
        });

    private async Task CreateTransaction(string categoryName, string accountName, DateOnly date, Money amount)
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
