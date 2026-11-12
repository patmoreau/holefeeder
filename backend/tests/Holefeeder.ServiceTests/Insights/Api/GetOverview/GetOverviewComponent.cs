using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.Insights.Api.GetOverview;

[Collection(ComponentTestMarker.Name)]
public class GetOverviewComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Now);
    private static readonly DateOnly CurrentMonth = new(Today.Year, Today.Month, 1);

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;

    [Fact]
    public Task WhenTheUserHasEarnedAndSpent() =>
        ScenarioRunner.Create("Getting the insights overview", _scenarioOutput)
            .Given("a registered user with monthly settings", ARegisteredUserWithMonthlySettings)
            .And("the user earned and spent over the last three months", TheUserEarnedAndSpentOverTheLastThreeMonths)
            .When("the user gets the overview of three periods", () => GettingTheOverview("?periods=3"))
            .Then<HttpResponseMessage>("the period's net and the last three periods are returned", TheNetAndTheLastThreePeriodsAreReturned)
            .PlayAsync();

    [Fact]
    public Task WhenTheCountIsNotOffered() =>
        ScenarioRunner.Create("Getting the insights overview of a count that is not offered", _scenarioOutput)
            .Given("a registered user with monthly settings", ARegisteredUserWithMonthlySettings)
            .When("the user gets the overview of four periods", () => GettingTheOverview("?periods=4"))
            .Then<HttpResponseMessage>("the request is rejected", TheRequestIsRejected)
            .PlayAsync();

    private async Task ARegisteredUserWithMonthlySettings()
    {
        _user = await AUser().SavedInDbAsync(server.Database);
        await new BudgetForTest(_user, server.Database).SettingsAsync(
            $$"""{"effectiveDate":"{{Today.Year - 1}}-01-01","intervalType":"monthly","frequency":1}""");
    }

    private async Task TheUserEarnedAndSpentOverTheLastThreeMonths()
    {
        var budget = new BudgetForTest(_user, server.Database);
        var salary = await budget.ACategoryAsync(CategoryType.Gain);
        var food = await budget.ACategoryAsync(CategoryType.Expense);
        var transferIn = await budget.ACategoryAsync(CategoryType.Gain, system: true);
        var transferOut = await budget.ACategoryAsync(CategoryType.Expense, system: true);
        await budget.ATransactionAsync(salary, CurrentMonth.AddMonths(-3), 9999m);
        await budget.ATransactionAsync(salary, CurrentMonth.AddMonths(-2), 1000m);
        await budget.ATransactionAsync(food, CurrentMonth.AddMonths(-2), 1200m);
        await budget.ATransactionAsync(salary, CurrentMonth, 3000m);
        await budget.ATransactionAsync(food, CurrentMonth, 2222.22m);
        await budget.ATransactionAsync(transferIn, CurrentMonth, 700m);
        await budget.ATransactionAsync(transferOut, CurrentMonth, 800m);
    }

    private async Task<HttpResponseMessage> GettingTheOverview(string query)
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri($"/api/v2/insights/overview{query}", UriKind.Relative));
    }

    private static async Task TheNetAndTheLastThreePeriodsAreReturned(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        var overview = await response.Value.Content.ReadFromJsonAsync<OverviewJson>();
        overview.Should().BeEquivalentTo(
            new OverviewJson(
                MonthOf(CurrentMonth), 3000m, 2222.22m, 777.78m, "saved", 26,
                [
                    new PeriodFlowJson(MonthOf(CurrentMonth.AddMonths(-2)), 1000m, 1200m, -200m),
                    new PeriodFlowJson(MonthOf(CurrentMonth.AddMonths(-1)), 0m, 0m, 0m),
                    new PeriodFlowJson(MonthOf(CurrentMonth), 3000m, 2222.22m, 777.78m)
                ],
                4000m, 3422.22m, 577.78m, new RangeJson("month", 3),
                [new RangeOptionJson(3, new RangeJson("month", 3)), new RangeOptionJson(6, new RangeJson("month", 6)), new RangeOptionJson(12, new RangeJson("year", 1))]),
            options => options.WithStrictOrdering());
    }

    private static void TheRequestIsRejected(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private static PeriodJson MonthOf(DateOnly start) => new(start, start.AddMonths(1).AddDays(-1));

    private sealed record OverviewJson(
        PeriodJson Period,
        decimal Income,
        decimal Spending,
        decimal Net,
        string Outcome,
        int SavedPercentage,
        IReadOnlyList<PeriodFlowJson> Series,
        decimal TotalIncome,
        decimal TotalSpending,
        decimal TotalNet,
        RangeJson Range,
        IReadOnlyList<RangeOptionJson> Ranges);

    private sealed record PeriodJson(DateOnly Start, DateOnly End);

    private sealed record PeriodFlowJson(PeriodJson Period, decimal Income, decimal Spending, decimal Net);

    private sealed record RangeJson(string Unit, int Amount);

    private sealed record RangeOptionJson(int Periods, RangeJson Range);
}
