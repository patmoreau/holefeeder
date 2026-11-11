using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Dashboard.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.Dashboard.TestKit;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

namespace Holefeeder.ServiceTests.Dashboard.Api.GetDashboardSummary;

[Collection(ComponentTestMarker.Name)]
public class GetDashboardSummaryComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Now);
    private static readonly DateOnly CurrentMonth = new(Today.Year, Today.Month, 1);

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;

    [Fact]
    public Task WhenTheUserHasTransactions() =>
        ScenarioRunner.Create("Getting the dashboard summary", _scenarioOutput)
            .Given("a registered user with monthly settings", ARegisteredUserWithMonthlySettings)
            .And("the user spent and earned over the last three months", TheUserSpentAndEarnedOverTheLastThreeMonths)
            .When("the user gets the dashboard summary", GettingTheDashboardSummary)
            .Then<HttpResponseMessage>("the current period is compared with the average", TheCurrentPeriodIsComparedWithTheAverage)
            .PlayAsync();

    [Fact]
    public Task WhenAnotherUserHasTransactions() =>
        ScenarioRunner.Create("Getting the dashboard summary while another user has transactions", _scenarioOutput)
            .Given("a registered user with monthly settings", ARegisteredUserWithMonthlySettings)
            .And("another user spent this month", AnotherUserSpentThisMonth)
            .When("the user gets the dashboard summary", GettingTheDashboardSummary)
            .Then<HttpResponseMessage>("an empty summary is returned", AnEmptySummaryIsReturned)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsNotRegistered() =>
        ScenarioRunner.Create("Getting the dashboard summary as a user who never registered", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .And("another user spent this month", AnotherUserSpentThisMonth)
            .When("the user gets the dashboard summary", GettingTheDashboardSummary)
            .Then<HttpResponseMessage>("an empty summary is returned", AnEmptySummaryIsReturned)
            .PlayAsync();

    private async Task ARegisteredUserWithMonthlySettings()
    {
        _user = await AUser().SavedInDbAsync(server.Database);
        await new BudgetForTest(_user, server.Database).SettingsAsync(
            $$"""{"effectiveDate":"{{Today.Year - 1}}-01-01","intervalType":"monthly","frequency":1}""");
    }

    private void AnUnregisteredUser() => _user = AUser();

    private async Task TheUserSpentAndEarnedOverTheLastThreeMonths()
    {
        var budget = new BudgetForTest(_user, server.Database);
        var purchase = await budget.ACategoryAsync(CategoryType.Expense);
        var food = await budget.ACategoryAsync(CategoryType.Expense);
        var transferOut = await budget.ACategoryAsync(CategoryType.Expense, system: true);
        var income = await budget.ACategoryAsync(CategoryType.Gain);
        var transferIn = await budget.ACategoryAsync(CategoryType.Gain, system: true);

        await budget.ATransactionAsync(purchase, CurrentMonth, 500.5m);
        await budget.ATransactionAsync(food, CurrentMonth.AddMonths(-2), 100.1m);
        await budget.ATransactionAsync(food, CurrentMonth.AddMonths(-1), 200.2m);
        await budget.ATransactionAsync(food, CurrentMonth, 300.3m);
        await budget.ATransactionAsync(transferOut, CurrentMonth, 800.8m);
        await budget.ATransactionAsync(food, CurrentMonth.AddDays(9), 400.4m);
        await budget.ATransactionAsync(income, CurrentMonth.AddMonths(-2), 1000.1m);
        await budget.ATransactionAsync(income, CurrentMonth.AddMonths(-1), 2000.2m);
        await budget.ATransactionAsync(income, CurrentMonth, 3000.3m);
        await budget.ATransactionAsync(transferIn, CurrentMonth, 7000.7m);
    }

    private async Task AnotherUserSpentThisMonth()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        var budget = new BudgetForTest(otherUser, server.Database);
        await budget.ATransactionAsync(await budget.ACategoryAsync(CategoryType.Expense), Today, 99.99m);
    }

    private async Task<HttpResponseMessage> GettingTheDashboardSummary()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri("/api/v2/dashboard/summary", UriKind.Relative));
    }

    private static Task TheCurrentPeriodIsComparedWithTheAverage(Ensure<HttpResponseMessage> response) =>
        TheSummaryIs(response, new SummaryJson(1201.2m, 700.7m, 140m, 1799.1m, 3000.3m, 500.5m));

    private static Task AnEmptySummaryIsReturned(Ensure<HttpResponseMessage> response) =>
        TheSummaryIs(response, new SummaryJson(0, 0, 0, 0, 0, 0));

    private static async Task TheSummaryIs(Ensure<HttpResponseMessage> response, SummaryJson expected)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        var summary = await response.Value.Content.ReadFromJsonAsync<SummaryJson>();
        summary.Should().Be(expected);
    }

    private sealed record SummaryJson(
        decimal CurrentExpenses,
        decimal ExpenseVariation,
        decimal ExpenseVariationPercentage,
        decimal NetFlow,
        decimal CurrentGains,
        decimal AverageExpenses);
}
