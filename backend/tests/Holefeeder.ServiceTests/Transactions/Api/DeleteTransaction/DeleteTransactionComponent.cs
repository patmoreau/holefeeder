using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Transactions.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;
using Holefeeder.ServiceTests.TestKit.Persistence;
using Holefeeder.ServiceTests.Transactions.TestKit;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Holefeeder.ServiceTests.Transactions.Api.DeleteTransaction;

[Collection(ComponentTestMarker.Name)]
public class DeleteTransactionComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _chequing;
    private Guid _market;
    private Guid _bakery;

    [Fact]
    public Task WhenTheTransactionIsDeleted() =>
        ScenarioRunner.Create("Deleting a transaction", _scenarioOutput)
            .Given("a registered user who bought groceries twice", ARegisteredUserWhoBoughtGroceriesTwice)
            .When("the user deletes the first purchase", () => DeletingTheTransaction(_market))
            .Then<HttpResponseMessage>("only the other purchase remains", OnlyTheOtherPurchaseRemains)
            .PlayAsync();

    [Fact]
    public Task WhenTheTransactionIsSomeoneElses() =>
        ScenarioRunner.Create("Deleting someone else's transaction", _scenarioOutput)
            .Given("a registered user who bought groceries twice", ARegisteredUserWhoBoughtGroceriesTwice)
            .When("another user deletes the first purchase", DeletingTheTransactionAsAnotherUser)
            .Then<HttpResponseMessage>("the transaction is not found", TheTransactionIsNotFound)
            .PlayAsync();

    [Fact]
    public Task WhenTheIdIsEmpty() =>
        ScenarioRunner.Create("Deleting a transaction with an empty id", _scenarioOutput)
            .Given("a registered user who bought groceries twice", ARegisteredUserWhoBoughtGroceriesTwice)
            .When("the user deletes a transaction with an empty id", () => DeletingTheTransaction(Guid.Empty))
            .Then<HttpResponseMessage>("a validation error is returned on the id", AValidationErrorIsReturnedOnTheId)
            .PlayAsync();

    private async Task ARegisteredUserWhoBoughtGroceriesTwice()
    {
        _user = await AUser().SavedInDbAsync(server.Database);
        var budget = new BudgetForTest(_user, server.Database);
        _chequing = await budget.AnAccountAsync("Chequing", AccountType.Checking);
        var groceries = await budget.ACategoryAsync(CategoryType.Expense);
        _market = await budget.ATransactionAsync(groceries, new DateOnly(2026, 8, 28), 10m, _chequing);
        _bakery = await budget.ATransactionAsync(groceries, new DateOnly(2026, 8, 29), 20m, _chequing);
    }

    private async Task<HttpResponseMessage> DeletingTheTransaction(Guid id)
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.DeleteAsync(new Uri($"/api/v2/transactions/{id}", UriKind.Relative));
    }

    private async Task<HttpResponseMessage> DeletingTheTransactionAsAnotherUser()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        using var client = server.CreateClientFor(otherUser.IdentityObjectId);
        return await client.DeleteAsync(new Uri($"/api/v2/transactions/{_market}", UriKind.Relative));
    }

    private async Task OnlyTheOtherPurchaseRemains(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await server.Database.TransactionsInDbAsync(_chequing)).Select(transaction => transaction.Id).Should().Equal(_bakery);
    }

    private async Task TheTransactionIsNotFound(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Detail.Should().Be(TransactionErrors.NotFound.Description);
        (await server.Database.TransactionsInDbAsync(_chequing)).Should().HaveCount(2);
    }

    private static async Task AValidationErrorIsReturnedOnTheId(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem.Errors.Should().ContainKey(IdErrors.CodeInvalid);
    }
}
