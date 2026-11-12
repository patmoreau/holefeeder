using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

using static Holefeeder.ServiceTests.Cashflows.TestKit.CashflowForTest;

namespace Holefeeder.ServiceTests.Cashflows.Api.ListCashflows;

[Collection(ComponentTestMarker.Name)]
public class ListCashflowsComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _rent;
    private Guid _gym;
    private Guid _payroll;
    private Guid _housing;
    private Guid _health;
    private Guid _salary;

    [Fact]
    public Task WhenTheUserHasCashflows() =>
        ScenarioRunner.Create("Listing the cashflows to manage", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has cashflows", TheUserHasCashflows)
            .When("the user lists the cashflows", ListingTheCashflows)
            .Then<HttpResponseMessage>("the active cashflows are listed in order", TheActiveCashflowsAreListed)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsNotRegistered() =>
        ScenarioRunner.Create("Listing the cashflows as a user who never registered", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .When("the user lists the cashflows", ListingTheCashflows)
            .Then<HttpResponseMessage>("nothing is listed", NothingIsListed)
            .PlayAsync();

    private async Task ARegisteredUser() => _user = await AUser().SavedInDbAsync(server.Database);

    private void AnUnregisteredUser() => _user = AUser();

    private async Task TheUserHasCashflows()
    {
        var chequing = AnId();
        var housing = AnId();
        var health = AnId();
        var salary = AnId();
        _housing = housing.Value;
        _health = health.Value;
        _salary = salary.Value;
        var rent = await ACashflow(1450m, description: "Rent", tags: "home,rent", accountId: chequing, categoryId: housing)
            .SavedInDbAsync(_user, server.Database, "Housing", "Expense", "#5856D6", "Chequing");
        await rent.PaidInDbAsync(_user, server.Database, new DateOnly(2026, 2, 2), new DateOnly(2026, 2, 1));
        _rent = rent.Id.Value;
        _gym = (await ACashflow(45m, description: "", tags: "", accountId: chequing, categoryId: health, effectiveDate: new DateOnly(2026, 11, 2), intervalType: "Weekly", frequency: 2)
            .SavedInDbAsync(_user, server.Database, "Health", "Expense", "#30B0C7", "Chequing")).Id.Value;
        _payroll = (await ACashflow(2400m, description: "Payroll", tags: "", accountId: chequing, categoryId: salary, intervalType: "OneTime")
            .SavedInDbAsync(_user, server.Database, "Salary", "Gain", "#34C759", "Chequing")).Id.Value;
        await ACashflow(description: "Old", inactive: true).SavedInDbAsync(_user, server.Database);
    }

    private async Task<HttpResponseMessage> ListingTheCashflows()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri("/api/v2/cashflows/manage", UriKind.Relative));
    }

    private async Task TheActiveCashflowsAreListed(Ensure<HttpResponseMessage> response)
    {
        var cashflows = await TheCashflowsAsync(response);
        cashflows.Should().BeEquivalentTo(
            [
                new CashflowJson(_gym, "Health", "", _health, "Health", "#30B0C7", "expense", "Chequing", 45m, "weekly", 2,
                    new DateOnly(2026, 11, 2), new DateOnly(2026, 11, 2), []),
                new CashflowJson(_rent, "Rent", "Rent", _housing, "Housing", "#5856D6", "expense", "Chequing", 1450m, "monthly", 1,
                    new DateOnly(2026, 1, 1), new DateOnly(2026, 3, 1), ["home", "rent"]),
                new CashflowJson(_payroll, "Payroll", "Payroll", _salary, "Salary", "#34C759", "gain", "Chequing", 2400m, "oneTime", 1,
                    new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 1), [])
            ],
            options => options.WithStrictOrdering());
    }

    private static async Task NothingIsListed(Ensure<HttpResponseMessage> response) =>
        (await TheCashflowsAsync(response)).Should().BeEmpty();

    private static async Task<List<CashflowJson>> TheCashflowsAsync(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        return (await response.Value.Content.ReadFromJsonAsync<List<CashflowJson>>())!;
    }

    private sealed record CashflowJson(
        Guid Id, string Title, string Description, Guid CategoryId, string CategoryName, string CategoryColor, string CategoryType, string AccountName,
        decimal Amount, string IntervalType, int Frequency, DateOnly EffectiveDate, DateOnly? NextDate, string[] Tags);
}
