using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.Insights.Api.GetIncome;

[Collection(ComponentTestMarker.Name)]
public class GetIncomeComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Now);
    private static readonly DateOnly CurrentMonth = new(Today.Year, Today.Month, 1);

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _salary;
    private Guid _unused;

    [Fact]
    public Task WhenTheUserHasEarned() =>
        ScenarioRunner.Create("Getting the income insights", _scenarioOutput)
            .Given("a registered user with monthly settings", ARegisteredUserWithMonthlySettings)
            .And("the user was paid last month and this month", TheUserWasPaidLastMonthAndThisMonth)
            .When("the user gets the income insights", GettingTheIncomeInsights)
            .Then<HttpResponseMessage>("this period's income is broken down", ThisPeriodsIncomeIsBrokenDown)
            .PlayAsync();

    [Fact]
    public Task WhenAnotherUserHasEarned() =>
        ScenarioRunner.Create("Getting the income insights while another user has earned", _scenarioOutput)
            .Given("a registered user with monthly settings", ARegisteredUserWithMonthlySettings)
            .And("another user earned this month", AnotherUserEarnedThisMonth)
            .When("the user gets the income insights", GettingTheIncomeInsights)
            .Then<HttpResponseMessage>("an empty breakdown is returned", AnEmptyBreakdownIsReturned)
            .PlayAsync();

    private async Task ARegisteredUserWithMonthlySettings()
    {
        _user = await AUser().SavedInDbAsync(server.Database);
        await new BudgetForTest(_user, server.Database).SettingsAsync(
            $$"""{"effectiveDate":"{{Today.Year - 1}}-01-01","intervalType":"monthly","frequency":1}""");
    }

    private async Task TheUserWasPaidLastMonthAndThisMonth()
    {
        var budget = new BudgetForTest(_user, server.Database);
        _salary = await budget.ACategoryAsync(CategoryType.Gain, name: "Salary", color: "#34C759");
        _unused = await budget.ACategoryAsync(CategoryType.Gain, name: "Unused", color: "#FF2D55");
        var groceries = await budget.ACategoryAsync(CategoryType.Expense, name: "Groceries");
        var transfer = await budget.ACategoryAsync(CategoryType.Gain, system: true);
        await budget.ATransactionAsync(_salary, CurrentMonth.AddMonths(-1), 3000m, tags: "job");
        await budget.ATransactionAsync(_salary, CurrentMonth, 3500m, tags: "job");
        await budget.ATransactionAsync(groceries, CurrentMonth, 200m, tags: "weekly");
        await budget.ATransactionAsync(transfer, CurrentMonth, 800m);
    }

    private async Task AnotherUserEarnedThisMonth()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        var budget = new BudgetForTest(otherUser, server.Database);
        await budget.ATransactionAsync(await budget.ACategoryAsync(CategoryType.Gain), Today, 99.99m, tags: "theirs");
    }

    private async Task<HttpResponseMessage> GettingTheIncomeInsights()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri("/api/v2/insights/income", UriKind.Relative));
    }

    private Task ThisPeriodsIncomeIsBrokenDown(Ensure<HttpResponseMessage> response) =>
        TheBreakdownIs(response, new BreakdownJson(
            ThisMonth(), 3500m, 3250m, 250m, 7.69m,
            [
                new CategoryJson(_salary, "Salary", "#34C759", 0m, "none", 0, 3500m, 3000m, [new TagAmountJson("job", 3500m)]),
                new CategoryJson(_unused, "Unused", "#FF2D55", 0m, "none", 0, 0m, 0m, [])
            ],
            [new TagJson("job", 3500m, 3000m)]));

    private static Task AnEmptyBreakdownIsReturned(Ensure<HttpResponseMessage> response) =>
        TheBreakdownIs(response, new BreakdownJson(ThisMonth(), 0m, 0m, 0m, 0m, [], []));

    private static PeriodJson ThisMonth() => new(CurrentMonth, CurrentMonth.AddMonths(1).AddDays(-1));

    private static async Task TheBreakdownIs(Ensure<HttpResponseMessage> response, BreakdownJson expected)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        var breakdown = await response.Value.Content.ReadFromJsonAsync<BreakdownJson>();
        breakdown.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    private sealed record BreakdownJson(
        PeriodJson Period,
        decimal Current,
        decimal Average,
        decimal Variation,
        decimal VariationPercentage,
        IReadOnlyList<CategoryJson> Categories,
        IReadOnlyList<TagJson> Tags);

    private sealed record PeriodJson(DateOnly Start, DateOnly End);

    private sealed record CategoryJson(
        Guid Id, string Name, string Color, decimal Budget, string BudgetStatus, int BudgetPercentage, decimal Amount, decimal Average, IReadOnlyList<TagAmountJson> Tags);

    private sealed record TagJson(string Tag, decimal Amount, decimal Average);

    private sealed record TagAmountJson(string Tag, decimal Amount);
}
