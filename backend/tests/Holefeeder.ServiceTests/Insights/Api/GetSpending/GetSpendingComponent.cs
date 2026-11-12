using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.Insights.Api.GetSpending;

[Collection(ComponentTestMarker.Name)]
public class GetSpendingComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Now);
    private static readonly DateOnly CurrentMonth = new(Today.Year, Today.Month, 1);

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _groceries;
    private Guid _unused;

    [Fact]
    public Task WhenTheUserHasSpent() =>
        ScenarioRunner.Create("Getting the spending insights", _scenarioOutput)
            .Given("a registered user with monthly settings", ARegisteredUserWithMonthlySettings)
            .And("the user spent on groceries last month and this month", TheUserSpentOnGroceriesLastMonthAndThisMonth)
            .When("the user gets the spending insights", GettingTheSpendingInsights)
            .Then<HttpResponseMessage>("this period's spending is broken down", ThisPeriodsSpendingIsBrokenDown)
            .PlayAsync();

    [Fact]
    public Task WhenAnotherUserHasSpent() =>
        ScenarioRunner.Create("Getting the spending insights while another user has spent", _scenarioOutput)
            .Given("a registered user with monthly settings", ARegisteredUserWithMonthlySettings)
            .And("another user spent this month", AnotherUserSpentThisMonth)
            .When("the user gets the spending insights", GettingTheSpendingInsights)
            .Then<HttpResponseMessage>("an empty breakdown is returned", AnEmptyBreakdownIsReturned)
            .PlayAsync();

    private async Task ARegisteredUserWithMonthlySettings()
    {
        _user = await AUser().SavedInDbAsync(server.Database);
        await new BudgetForTest(_user, server.Database).SettingsAsync(
            $$"""{"effectiveDate":"{{Today.Year - 1}}-01-01","intervalType":"monthly","frequency":1}""");
    }

    private async Task TheUserSpentOnGroceriesLastMonthAndThisMonth()
    {
        var budget = new BudgetForTest(_user, server.Database);
        _groceries = await budget.ACategoryAsync(CategoryType.Expense, name: "Groceries", color: "#34C759", budget: 300m);
        _unused = await budget.ACategoryAsync(CategoryType.Expense, name: "Unused", color: "#FF2D55");
        var transfer = await budget.ACategoryAsync(CategoryType.Expense, system: true);
        await budget.ATransactionAsync(_groceries, CurrentMonth.AddMonths(-1), 100m, tags: "weekly");
        await budget.ATransactionAsync(_groceries, CurrentMonth, 40m, tags: "weekly");
        await budget.ATransactionAsync(transfer, CurrentMonth, 800m);
    }

    private async Task AnotherUserSpentThisMonth()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        var budget = new BudgetForTest(otherUser, server.Database);
        await budget.ATransactionAsync(await budget.ACategoryAsync(CategoryType.Expense), Today, 99.99m, tags: "theirs");
    }

    private async Task<HttpResponseMessage> GettingTheSpendingInsights()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri("/api/v2/insights/spending", UriKind.Relative));
    }

    private Task ThisPeriodsSpendingIsBrokenDown(Ensure<HttpResponseMessage> response) =>
        TheBreakdownIs(response, new BreakdownJson(
            ThisMonth(), 40m, 70m, -30m, -42.86m,
            [
                new CategoryJson(_groceries, "Groceries", "#34C759", 300m, 40m, 100m, [new TagAmountJson("weekly", 40m)]),
                new CategoryJson(_unused, "Unused", "#FF2D55", 0m, 0m, 0m, [])
            ],
            [new TagJson("weekly", 40m, 100m)]));

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
        Guid Id, string Name, string Color, decimal Budget, decimal Amount, decimal Average, IReadOnlyList<TagAmountJson> Tags);

    private sealed record TagJson(string Tag, decimal Amount, decimal Average);

    private sealed record TagAmountJson(string Tag, decimal Amount);
}
