using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Transactions.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;
using Holefeeder.ServiceTests.TestKit.Persistence;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Holefeeder.ServiceTests.Transactions.Api.GetTransaction;

[Collection(ComponentTestMarker.Name)]
public class GetTransactionComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private static readonly DateOnly Bought = new(2026, 8, 28);

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _chequing;
    private Guid _groceries;
    private Guid _transaction;

    [Fact]
    public Task WhenTheTransactionExists() =>
        ScenarioRunner.Create("Getting a transaction", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user bought groceries", TheUserBoughtGroceries)
            .When("the user gets the transaction", () => GettingTheTransaction(_transaction))
            .Then<HttpResponseMessage>("the transaction comes with its account and category", TheTransactionComesWithItsAccountAndCategory)
            .PlayAsync();

    [Fact]
    public Task WhenTheTransactionBelongsToAnotherUser() =>
        ScenarioRunner.Create("Getting another user's transaction", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("another user bought groceries", AnotherUserBoughtGroceries)
            .When("the user gets the other user's transaction", () => GettingTheTransaction(_transaction))
            .Then<HttpResponseMessage>("the transaction is not found", TheTransactionIsNotFound)
            .PlayAsync();

    [Fact]
    public Task WhenTheIdIsEmpty() =>
        ScenarioRunner.Create("Getting a transaction with an empty id", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user gets a transaction with an empty id", () => GettingTheTransaction(Guid.Empty))
            .Then<HttpResponseMessage>("a validation error is returned on the id", AValidationErrorIsReturnedOnTheId)
            .PlayAsync();

    private async Task ARegisteredUser() => _user = await AUser().SavedInDbAsync(server.Database);

    private async Task TheUserBoughtGroceries()
    {
        var budget = new BudgetForTest(_user, server.Database);
        _chequing = await budget.AnAccountAsync("Chequing", AccountType.Checking);
        _groceries = await budget.ACategoryAsync(CategoryType.Expense, name: "Groceries", color: "#34C759");
        _transaction = await budget.ATransactionAsync(_groceries, Bought, 123.45m, _chequing, description: "Market", tags: "weekly,family");
    }

    private async Task AnotherUserBoughtGroceries()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        var budget = new BudgetForTest(otherUser, server.Database);
        _transaction = await budget.ATransactionAsync(await budget.ACategoryAsync(CategoryType.Expense), Bought, 10m);
    }

    private async Task<HttpResponseMessage> GettingTheTransaction(Guid id)
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri($"/api/v2/transactions/{id}", UriKind.Relative));
    }

    private async Task TheTransactionComesWithItsAccountAndCategory(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        var transaction = await response.Value.Content.ReadFromJsonAsync<TransactionJson>();
        transaction.Should().BeEquivalentTo(new TransactionJson(
            _transaction, Bought, 123.45m, "Market", ["weekly", "family"],
            new CategoryJson(_groceries, "Groceries", "expense", "#34C759"),
            new AccountJson(_chequing, "Chequing")));
    }

    private static async Task TheTransactionIsNotFound(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Detail.Should().Be(TransactionErrors.NotFound.Description);
    }

    private static async Task AValidationErrorIsReturnedOnTheId(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem.Errors.Should().ContainKey(IdErrors.CodeInvalid);
    }

    private sealed record TransactionJson(
        Guid Id, DateOnly Date, decimal Amount, string Description, IReadOnlyList<string> Tags, CategoryJson Category, AccountJson Account);

    private sealed record CategoryJson(Guid Id, string Name, string Type, string Color);

    private sealed record AccountJson(Guid Id, string Name);
}
