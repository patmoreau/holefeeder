using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Dashboard.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.Dashboard.TestKit;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

namespace Holefeeder.ServiceTests.Dashboard.Api.GetUpcomingFlows;

[Collection(ComponentTestMarker.Name)]
public class GetUpcomingFlowsComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Now);
    private static readonly DateOnly CurrentMonth = new(Today.Year, Today.Month, 1);

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _rent;
    private Guid _phone;

    [Fact]
    public Task WhenTheUserHasCashflows() =>
        ScenarioRunner.Create("Getting the dashboard upcoming flows", _scenarioOutput)
            .Given("a registered user with monthly settings", ARegisteredUserWithMonthlySettings)
            .And("the user has cashflows due this month and later", TheUserHasCashflowsDueThisMonthAndLater)
            .And("another user has a cashflow", AnotherUserHasACashflow)
            .When("the user gets the upcoming flows", GettingTheUpcomingFlows)
            .Then<HttpResponseMessage>("the flows due this period are returned", TheFlowsDueThisPeriodAreReturned)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsNotRegistered() =>
        ScenarioRunner.Create("Getting the dashboard upcoming flows as a user who never registered", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .And("another user has a cashflow", AnotherUserHasACashflow)
            .When("the user gets the upcoming flows", GettingTheUpcomingFlows)
            .Then<HttpResponseMessage>("no flow is returned", NoFlowIsReturned)
            .PlayAsync();

    private async Task ARegisteredUserWithMonthlySettings()
    {
        _user = await AUser().SavedInDbAsync(server.Database);
        await new BudgetForTest(_user, server.Database).SettingsAsync(
            $$"""{"effectiveDate":"{{Today.Year - 1}}-01-01","intervalType":"monthly","frequency":1}""");
    }

    private void AnUnregisteredUser() => _user = AUser();

    private async Task TheUserHasCashflowsDueThisMonthAndLater()
    {
        var budget = new BudgetForTest(_user, server.Database);
        var housing = await budget.ACategoryAsync(CategoryType.Expense, name: "Housing", color: "#007AFF");
        var utilities = await budget.ACategoryAsync(CategoryType.Expense, name: "Utilities", color: "#34C759");
        _rent = await budget.ACashflowAsync(housing, CurrentMonth, IntervalType.Monthly, 1, 1450m, description: "Rent", tags: "housing");
        _phone = await budget.ACashflowAsync(utilities, Today, IntervalType.OneTime, 1, 65m, tags: "phone");
        await budget.ACashflowAsync(utilities, CurrentMonth.AddMonths(1), IntervalType.OneTime, 1, 88.4m, description: "Next month");
        await budget.ACashflowAsync(utilities, Today, IntervalType.OneTime, 1, 12m, inactive: true, description: "Cancelled");
    }

    private async Task AnotherUserHasACashflow()
    {
        var budget = new BudgetForTest(await AUser().SavedInDbAsync(server.Database), server.Database);
        await budget.ACashflowAsync(await budget.ACategoryAsync(CategoryType.Expense), Today, IntervalType.OneTime, 1, 99.99m);
    }

    private async Task<HttpResponseMessage> GettingTheUpcomingFlows()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri("/api/v2/dashboard/upcoming", UriKind.Relative));
    }

    private Task TheFlowsDueThisPeriodAreReturned(Ensure<HttpResponseMessage> response) =>
        TheFlowsAre(response,
            new FlowJson(_rent, CurrentMonth, "Rent", "Housing", "#007AFF", ["housing"], 1450m, "expense"),
            new FlowJson(_phone, Today, "Utilities", null, "#34C759", ["phone"], 65m, "expense"));

    private static Task NoFlowIsReturned(Ensure<HttpResponseMessage> response) => TheFlowsAre(response);

    private static async Task TheFlowsAre(Ensure<HttpResponseMessage> response, params FlowJson[] expected)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        var flows = await response.Value.Content.ReadFromJsonAsync<List<FlowJson>>();
        flows.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    private sealed record FlowJson(
        Guid CashflowId, DateOnly Date, string Title, string? Category, string CategoryColor, string[] Tags, decimal Amount, string CategoryType);
}
