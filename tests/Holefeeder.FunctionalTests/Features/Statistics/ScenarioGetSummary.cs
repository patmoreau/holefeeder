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

public class ScenarioGetSummary(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper) : HolefeederScenario(applicationDriver, testOutputHelper)
{
    private readonly UserId _userId = TestUsers[AuthorizedUser].UserId;

    private readonly Dictionary<string, Category> _categories = [];
    private readonly Dictionary<string, Account> _accounts = [];

    [Fact]
    public Task WhenGettingSummaryStatistics() =>
        ScenarioRunner.Create("A user sends was to know his statistics", ScenarioOutput)
            .Given("a 'purchase' transaction was made on the 'credit card' account in February 2023", () => CreateTransaction("purchase", "credit card", new DateOnly(2023, 2, 1), Money.Create(500.5m).Value))
            .And("a 'food and drink' transaction was made on the 'credit card' account in December 2022", () => CreateTransaction("food and drink", "credit card", new DateOnly(2022, 12, 1), Money.Create(100.1m).Value))
            .And("a 'food and drink' transaction was made on the 'credit card' account in January 2023", () => CreateTransaction("food and drink", "credit card", new DateOnly(2023, 1, 1), Money.Create(200.2m).Value))
            .And("a 'food and drink' transaction was made on the 'credit card' account in February 2023", () => CreateTransaction("food and drink", "credit card", new DateOnly(2023, 2, 1), Money.Create(300.3m).Value))
            .And("a second 'food and drink' transaction was made on the 'checking' account in February 2023", () => CreateTransaction("food and drink", "checking", new DateOnly(2023, 2, 10), Money.Create(400.4m).Value))
            .And("an 'income' gain was made to the 'checking' account in December 2022", () => CreateGain("income", "checking", new DateOnly(2022, 12, 1), Money.Create(1000.1m).Value))
            .And("an 'income' gain was made to the 'checking' account in January 2023", () => CreateGain("income", "checking", new DateOnly(2023, 1, 1), Money.Create(2000.2m).Value))
            .And("an 'income' gain was made to the 'checking' account in February 2023", () => CreateGain("income", "checking", new DateOnly(2023, 2, 1), Money.Create(3000.3m).Value))
            .When(TheUserGetsTheirFebruarySummaryStatistics)
            .Then(TheSummaryShouldMatchTheExpected)
            .PlayAsync();

    private void TheUserGetsTheirFebruarySummaryStatistics(IStepRunner runner) =>
        TheUser.GetSummaryStatisticsForRange(runner, new DateOnly(2023, 2, 1), new DateOnly(2023, 2, 28));

    private static void TheSummaryShouldMatchTheExpected(IStepRunner runner) =>
        runner.Execute<IApiResponse<SummaryDto>>(response =>
        {
            var expectedSummary = new SummaryDto(
                new SummaryValue(2000.2m, 200.2m),
                new SummaryValue(3000.3m, 1201.2m),
                new SummaryValue(2000.2m, 500.5m));

            response.Should().BeValid()
                .And.Subject.Value.Should().BeSuccessful()
                .And.HaveContent();
            var results = response.Value.Content;
            results.Should()
                .NotBeNull()
                .And.BeEquivalentTo(expectedSummary);
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

    private async Task CreateGain(string categoryName, string accountName, DateOnly date, Money amount)
    {
        if (!_categories.TryGetValue(categoryName, out var category))
        {
            category = await CategoryBuilder.GivenACategory().OfType(CategoryType.Gain).WithName(categoryName).ForUser(_userId).SavedInDbAsync(DatabaseDriver);
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
