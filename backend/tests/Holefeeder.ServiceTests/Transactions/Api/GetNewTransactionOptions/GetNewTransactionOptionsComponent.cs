using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Dashboard.Core.GetDashboardAccounts;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.Dashboard.TestKit;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

namespace Holefeeder.ServiceTests.Transactions.Api.GetNewTransactionOptions;

[Collection(ComponentTestMarker.Name)]
public class GetNewTransactionOptionsComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _chequing;
    private Guid _savings;
    private Guid _groceries;
    private Guid _salary;

    [Fact]
    public Task WhenTheUserHasAccountsAndCategories() =>
        ScenarioRunner.Create("Getting the new transaction options", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has accounts and categories", TheUserHasAccountsAndCategories)
            .When("the user gets the new transaction options", GettingTheOptions)
            .Then<HttpResponseMessage>("the open accounts and usable categories are listed in order", TheOptionsAreListedInOrder)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsNotRegistered() =>
        ScenarioRunner.Create("Getting the new transaction options as a user who never registered", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .When("the user gets the new transaction options", GettingTheOptions)
            .Then<HttpResponseMessage>("nothing is listed", NothingIsListed)
            .PlayAsync();

    private async Task ARegisteredUser() => _user = await AUser().SavedInDbAsync(server.Database);

    private void AnUnregisteredUser() => _user = AUser();

    private async Task TheUserHasAccountsAndCategories()
    {
        var budget = new BudgetForTest(_user, server.Database);
        _savings = await budget.AnAccountAsync("Savings", AccountType.Savings);
        _chequing = await budget.AnAccountAsync("Chequing", AccountType.Checking, favorite: true);
        await budget.AnAccountAsync("Closed", AccountType.Checking, inactive: true);
        _salary = await budget.ACategoryAsync(CategoryType.Gain, name: "Salary", color: "#007AFF");
        _groceries = await budget.ACategoryAsync(CategoryType.Expense, name: "Groceries", color: "#34C759");
        await budget.ACategoryAsync(CategoryType.Expense, system: true, name: "Transfer Out");
    }

    private async Task<HttpResponseMessage> GettingTheOptions()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri("/api/v2/transactions/new-transaction-options", UriKind.Relative));
    }

    private async Task TheOptionsAreListedInOrder(Ensure<HttpResponseMessage> response)
    {
        var options = await TheOptionsAsync(response);
        options.Accounts.Should().Equal(new AccountJson(_chequing, "Chequing"), new AccountJson(_savings, "Savings"));
        options.Categories.Should().Equal(
            new CategoryJson(_groceries, "Groceries", "#34C759", "expense"),
            new CategoryJson(_salary, "Salary", "#007AFF", "gain"));
    }

    private static async Task NothingIsListed(Ensure<HttpResponseMessage> response)
    {
        var options = await TheOptionsAsync(response);
        options.Accounts.Should().BeEmpty();
        options.Categories.Should().BeEmpty();
    }

    private static async Task<OptionsJson> TheOptionsAsync(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        return (await response.Value.Content.ReadFromJsonAsync<OptionsJson>())!;
    }

    private sealed record OptionsJson(List<AccountJson> Accounts, List<CategoryJson> Categories);

    private sealed record AccountJson(Guid Id, string Name);

    private sealed record CategoryJson(Guid Id, string Name, string Color, string Type);
}
