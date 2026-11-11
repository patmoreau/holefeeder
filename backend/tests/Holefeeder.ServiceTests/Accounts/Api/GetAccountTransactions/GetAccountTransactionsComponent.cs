using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Shared.Core.Search;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;
using Holefeeder.ServiceTests.TestKit.Persistence;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Holefeeder.ServiceTests.Accounts.Api.GetAccountTransactions;

[Collection(ComponentTestMarker.Name)]
public class GetAccountTransactionsComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _chequing;
    private Guid _lunch;
    private Guid _payCheck;

    [Fact]
    public Task WhenTheAccountHasTransactions() =>
        ScenarioRunner.Create("Getting a page of an account's transactions", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user's chequing account has three transactions", TheChequingAccountHasThreeTransactions)
            .When("the user gets the first two transactions", () => GettingTheTransactions(_chequing, "?offset=0&limit=2"))
            .Then<HttpResponseMessage>("the newest two are returned with the total", TheNewestTwoAreReturnedWithTheTotal)
            .PlayAsync();

    [Fact]
    public Task WhenTheAccountBelongsToAnotherUser() =>
        ScenarioRunner.Create("Getting the transactions of another user's account", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("another user has an account", AnotherUserHasAnAccount)
            .When("the user gets the transactions of that account", () => GettingTheTransactions(_chequing, string.Empty))
            .Then<HttpResponseMessage>("the account is not found", TheAccountIsNotFound)
            .PlayAsync();

    [Fact]
    public Task WhenThePageIsInvalid() =>
        ScenarioRunner.Create("Getting an account's transactions with an invalid page", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user asks for a negative offset and too many transactions",
                () => GettingTheTransactions(Guid.NewGuid(), "?offset=-1&limit=101"))
            .Then<HttpResponseMessage>("validation errors are returned on the page", ValidationErrorsAreReturnedOnThePage)
            .PlayAsync();

    private async Task ARegisteredUser()
    {
        _user = await AUser().SavedInDbAsync(server.Database);
        await new BudgetForTest(_user, server.Database).SettingsAsync("""{"effectiveDate":"2026-01-01","intervalType":"monthly","frequency":1}""");
    }

    private async Task TheChequingAccountHasThreeTransactions()
    {
        var budget = new BudgetForTest(_user, server.Database);
        _chequing = await budget.AnAccountAsync("Chequing", AccountType.Checking);
        var food = await budget.ACategoryAsync(CategoryType.Expense, name: "Food", color: "#FF8D28");
        var pay = await budget.ACategoryAsync(CategoryType.Gain, name: "Pay check", color: "#34C759");
        await budget.ATransactionAsync(food, new DateOnly(2026, 8, 26), 54.2m, _chequing, description: "Gas");
        _lunch = await budget.ATransactionAsync(food, new DateOnly(2026, 9, 13), 1.25m, _chequing, description: "Lunch", tags: "restaurant");
        _payCheck = await budget.ATransactionAsync(pay, new DateOnly(2026, 9, 12), 1000m, _chequing);
    }

    private async Task AnotherUserHasAnAccount()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        _chequing = await new BudgetForTest(otherUser, server.Database).AnAccountAsync("Chequing", AccountType.Checking);
    }

    private async Task<HttpResponseMessage> GettingTheTransactions(Guid accountId, string query)
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri($"/api/v2/accounts/{accountId}/transactions{query}", UriKind.Relative));
    }

    private async Task TheNewestTwoAreReturnedWithTheTotal(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Value.Headers.GetValues("X-Total-Count").Should().Equal("3");
        var transactions = await response.Value.Content.ReadFromJsonAsync<List<TransactionJson>>();
        transactions.Should().BeEquivalentTo([
            new TransactionJson(_lunch, "Lunch", "Food", "#FF8D28", ["restaurant"], 1.25m, new DateOnly(2026, 9, 13)),
            new TransactionJson(_payCheck, "Pay check", null, "#34C759", [], 1000m, new DateOnly(2026, 9, 12))
        ], options => options.WithStrictOrdering());
    }

    private static async Task TheAccountIsNotFound(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Detail.Should().Be(AccountErrors.NotFound.Description);
    }

    private static async Task ValidationErrorsAreReturnedOnThePage(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem.Errors.Keys.Should().BeEquivalentTo(OffsetErrors.CodeInvalid, LimitErrors.CodeInvalid);
    }

    private sealed record TransactionJson(
        Guid Id, string Title, string? Category, string CategoryColor, IReadOnlyList<string> Tags, decimal Amount, DateOnly Date);
}
