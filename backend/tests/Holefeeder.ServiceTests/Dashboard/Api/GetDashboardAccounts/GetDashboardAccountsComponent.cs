using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Dashboard.Core;
using Holefeeder.Service.Dashboard.Core.GetDashboardAccounts;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.Dashboard.TestKit;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

namespace Holefeeder.ServiceTests.Dashboard.Api.GetDashboardAccounts;

[Collection(ComponentTestMarker.Name)]
public class GetDashboardAccountsComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Now);
    private static readonly DateOnly CurrentMonth = new(Today.Year, Today.Month, 1);
    private static readonly DateOnly OpenDate = new(2025, 3, 1);

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _chequing;
    private Guid _amex;

    [Fact]
    public Task WhenTheUserHasAccounts() =>
        ScenarioRunner.Create("Getting the dashboard accounts", _scenarioOutput)
            .Given("a registered user with monthly settings", ARegisteredUserWithMonthlySettings)
            .And("the user has a chequing account and a credit card", TheUserHasAChequingAccountAndACreditCard)
            .And("another user has an account", AnotherUserHasAnAccount)
            .When("the user gets the dashboard accounts", GettingTheDashboardAccounts)
            .Then<HttpResponseMessage>("each account shows its balance and projection", EachAccountShowsItsBalanceAndProjection)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsNotRegistered() =>
        ScenarioRunner.Create("Getting the dashboard accounts as a user who never registered", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .And("another user has an account", AnotherUserHasAnAccount)
            .When("the user gets the dashboard accounts", GettingTheDashboardAccounts)
            .Then<HttpResponseMessage>("no account is returned", NoAccountIsReturned)
            .PlayAsync();

    private async Task ARegisteredUserWithMonthlySettings()
    {
        _user = await AUser().SavedInDbAsync(server.Database);
        await new BudgetForTest(_user, server.Database).SettingsAsync(
            $$"""{"effectiveDate":"{{Today.Year - 1}}-01-01","intervalType":"monthly","frequency":1}""");
    }

    private void AnUnregisteredUser() => _user = AUser();

    private async Task TheUserHasAChequingAccountAndACreditCard()
    {
        var budget = new BudgetForTest(_user, server.Database);
        _chequing = await budget.AnAccountAsync("Chequing", AccountType.Checking, openBalance: 100m, openDate: OpenDate, favorite: true);
        _amex = await budget.AnAccountAsync("Amex", AccountType.CreditCard, openDate: OpenDate);
        var food = await budget.ACategoryAsync(CategoryType.Expense);
        var salary = await budget.ACategoryAsync(CategoryType.Gain);
        await budget.ATransactionAsync(salary, CurrentMonth, 2500m, _chequing);
        await budget.ATransactionAsync(food, CurrentMonth, 613.59m, _chequing);
        await budget.ATransactionAsync(food, CurrentMonth, 123.45m, _amex);
        await budget.ACashflowAsync(food, Today, IntervalType.OneTime, 1, 66.67m, accountId: _amex);
    }

    private async Task AnotherUserHasAnAccount()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        await new BudgetForTest(otherUser, server.Database).AnAccountAsync("Other", AccountType.Checking);
    }

    private async Task<HttpResponseMessage> GettingTheDashboardAccounts()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri("/api/v2/dashboard/accounts", UriKind.Relative));
    }

    private Task EachAccountShowsItsBalanceAndProjection(Ensure<HttpResponseMessage> response) =>
        TheAccountsAre(response,
            new AccountJson(_chequing, "Chequing", "availableBalance", 1986.41m, 1986.41m, CurrentMonth),
            new AccountJson(_amex, "Amex", "amountDue", -123.45m, -190.12m, CurrentMonth));

    private static Task NoAccountIsReturned(Ensure<HttpResponseMessage> response) => TheAccountsAre(response);

    private static async Task TheAccountsAre(Ensure<HttpResponseMessage> response, params AccountJson[] expected)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        var accounts = await response.Value.Content.ReadFromJsonAsync<List<AccountJson>>();
        accounts.Should().Equal(expected);
    }

    private sealed record AccountJson(Guid Id, string Name, string Caption, decimal Balance, decimal ProjectedBalance, DateOnly Updated);
}
