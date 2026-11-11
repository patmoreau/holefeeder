using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Dashboard.Core;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.Dashboard.TestKit;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.Dashboard.Api.GetRecentTransactions;

[Collection(ComponentTestMarker.Name)]
public class GetRecentTransactionsComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _lunch;
    private Guid _payCheck;
    private Guid _coffee;
    private Guid _gas;

    [Fact]
    public Task WhenTheUserHasTransactions() =>
        ScenarioRunner.Create("Getting the dashboard recent transactions", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has five transactions", TheUserHasFiveTransactions)
            .And("another user has a transaction", AnotherUserHasATransaction)
            .When("the user gets the recent transactions", GettingTheRecentTransactions)
            .Then<HttpResponseMessage>("the four most recent transactions are returned", TheFourMostRecentTransactionsAreReturned)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsNotRegistered() =>
        ScenarioRunner.Create("Getting the dashboard recent transactions as a user who never registered", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .And("another user has a transaction", AnotherUserHasATransaction)
            .When("the user gets the recent transactions", GettingTheRecentTransactions)
            .Then<HttpResponseMessage>("no transaction is returned", NoTransactionIsReturned)
            .PlayAsync();

    private async Task ARegisteredUser() => _user = await AUser().SavedInDbAsync(server.Database);

    private void AnUnregisteredUser() => _user = AUser();

    private async Task TheUserHasFiveTransactions()
    {
        var budget = new BudgetForTest(_user, server.Database);
        var food = await budget.ACategoryAsync(CategoryType.Expense, name: "Food", color: "#FF8D28");
        var pay = await budget.ACategoryAsync(CategoryType.Gain, name: "Pay check", color: "#34C759");
        await budget.ATransactionAsync(food, new DateOnly(2026, 8, 1), 9.99m, description: "Oldest");
        _gas = await budget.ATransactionAsync(food, new DateOnly(2026, 8, 26), 54.2m, description: "Gas", tags: "car");
        _coffee = await budget.ATransactionAsync(food, new DateOnly(2026, 8, 28), 123.45m, tags: "restaurant");
        _payCheck = await budget.ATransactionAsync(pay, new DateOnly(2026, 9, 12), 1000m);
        _lunch = await budget.ATransactionAsync(food, new DateOnly(2026, 9, 13), 1.25m, description: "Lunch", tags: "restaurant,work");
    }

    private async Task AnotherUserHasATransaction()
    {
        var budget = new BudgetForTest(await AUser().SavedInDbAsync(server.Database), server.Database);
        await budget.ATransactionAsync(await budget.ACategoryAsync(CategoryType.Expense), new DateOnly(2026, 9, 30), 99.99m);
    }

    private async Task<HttpResponseMessage> GettingTheRecentTransactions()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri("/api/v2/dashboard/transactions", UriKind.Relative));
    }

    private Task TheFourMostRecentTransactionsAreReturned(Ensure<HttpResponseMessage> response) =>
        TheTransactionsAre(response,
            new TransactionJson(_lunch, "Lunch", "Food", "#FF8D28", ["restaurant", "work"], 1.25m, new DateOnly(2026, 9, 13)),
            new TransactionJson(_payCheck, "Pay check", null, "#34C759", [], 1000m, new DateOnly(2026, 9, 12)),
            new TransactionJson(_coffee, "Food", null, "#FF8D28", ["restaurant"], 123.45m, new DateOnly(2026, 8, 28)),
            new TransactionJson(_gas, "Gas", "Food", "#FF8D28", ["car"], 54.2m, new DateOnly(2026, 8, 26)));

    private static Task NoTransactionIsReturned(Ensure<HttpResponseMessage> response) => TheTransactionsAre(response);

    private static async Task TheTransactionsAre(Ensure<HttpResponseMessage> response, params TransactionJson[] expected)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        var transactions = await response.Value.Content.ReadFromJsonAsync<List<TransactionJson>>();
        transactions.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    private sealed record TransactionJson(
        Guid Id, string Title, string? Category, string CategoryColor, string[] Tags, decimal Amount, DateOnly Date);
}
