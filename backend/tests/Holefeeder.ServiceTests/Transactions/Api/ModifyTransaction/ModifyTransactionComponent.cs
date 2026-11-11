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

namespace Holefeeder.ServiceTests.Transactions.Api.ModifyTransaction;

[Collection(ComponentTestMarker.Name)]
public class ModifyTransactionComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _chequing;
    private Guid _savings;
    private Guid _groceries;
    private Guid _transferIn;
    private Guid _transaction;
    private object _body = null!;

    [Fact]
    public Task WhenTheTransactionIsModified() =>
        ScenarioRunner.Create("Modifying a transaction", _scenarioOutput)
            .Given("a registered user who bought groceries", ARegisteredUserWhoBoughtGroceries)
            .And("a modification moving it to savings as a transfer in", AModificationMovingItToSavingsAsATransferIn)
            .When("the user modifies the transaction", ModifyingTheTransaction)
            .Then<HttpResponseMessage>("every field is saved", EveryFieldIsSaved)
            .PlayAsync();

    [Fact]
    public Task WhenTheAccountIsSomeoneElses() =>
        ScenarioRunner.Create("Modifying a transaction onto someone else's account", _scenarioOutput)
            .Given("a registered user who bought groceries", ARegisteredUserWhoBoughtGroceries)
            .And("a modification onto another user's account", AModificationOntoAnotherUsersAccount)
            .When("the user modifies the transaction", ModifyingTheTransaction)
            .Then<HttpResponseMessage>("the account is reported missing", TheAccountIsReportedMissing)
            .PlayAsync();

    [Fact]
    public Task WhenTheTransactionIsSomeoneElses() =>
        ScenarioRunner.Create("Modifying someone else's transaction", _scenarioOutput)
            .Given("a registered user who bought groceries", ARegisteredUserWhoBoughtGroceries)
            .And("a modification of another user's transaction", AModificationOfAnotherUsersTransaction)
            .When("the user modifies the transaction", ModifyingTheTransaction)
            .Then<HttpResponseMessage>("the transaction is not found", TheTransactionIsNotFound)
            .PlayAsync();

    [Fact]
    public Task WhenTheModificationIsInvalid() =>
        ScenarioRunner.Create("Modifying a transaction without a date", _scenarioOutput)
            .Given("a registered user who bought groceries", ARegisteredUserWhoBoughtGroceries)
            .And("a modification without a date", AModificationWithoutADate)
            .When("the user modifies the transaction", ModifyingTheTransaction)
            .Then<HttpResponseMessage>("a validation error is returned on the date", AValidationErrorIsReturnedOnTheDate)
            .PlayAsync();

    private async Task ARegisteredUserWhoBoughtGroceries()
    {
        _user = await AUser().SavedInDbAsync(server.Database);
        var budget = new BudgetForTest(_user, server.Database);
        _chequing = await budget.AnAccountAsync("Chequing", AccountType.Checking);
        _savings = await budget.AnAccountAsync("Savings", AccountType.Savings);
        _groceries = await budget.ACategoryAsync(CategoryType.Expense, name: "Groceries");
        _transferIn = await budget.ACategoryAsync(CategoryType.Gain, system: true, name: "Transfer In");
        _transaction = await budget.ATransactionAsync(_groceries, new DateOnly(2026, 8, 28), 123.45m, _chequing, "Market", "weekly");
    }

    private void AModificationMovingItToSavingsAsATransferIn() =>
        _body = new
        {
            id = _transaction.ToString(),
            date = "2026-09-01",
            amount = 50.25m,
            description = "From chequing",
            accountId = _savings.ToString(),
            categoryId = _transferIn.ToString(),
            tags = new[] { "Family" }
        };

    private async Task AModificationOntoAnotherUsersAccount()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        var others = await new BudgetForTest(otherUser, server.Database).AnAccountAsync("Other", AccountType.Checking);
        _body = new
        {
            id = _transaction.ToString(),
            date = "2026-09-01",
            amount = 50m,
            accountId = others.ToString(),
            categoryId = _groceries.ToString()
        };
    }

    private async Task AModificationOfAnotherUsersTransaction()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        var budget = new BudgetForTest(otherUser, server.Database);
        var others = await budget.ATransactionAsync(await budget.ACategoryAsync(CategoryType.Expense), new DateOnly(2026, 8, 28), 10m);
        _body = new
        {
            id = others.ToString(),
            date = "2026-09-01",
            amount = 50m,
            accountId = _chequing.ToString(),
            categoryId = _groceries.ToString()
        };
    }

    private void AModificationWithoutADate() =>
        _body = new { id = _transaction.ToString(), amount = 50m, accountId = _chequing.ToString(), categoryId = _groceries.ToString() };

    private async Task<HttpResponseMessage> ModifyingTheTransaction()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.PostAsJsonAsync(new Uri("/api/v2/transactions/modify", UriKind.Relative), _body);
    }

    private async Task EveryFieldIsSaved(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await server.Database.TransactionsInDbAsync(_chequing)).Should().BeEmpty();
        (await server.Database.TransactionsInDbAsync(_savings)).Should().Equal(new TransactionInDb(
            _transaction, new DateOnly(2026, 9, 1), 50.25m, "From chequing", _savings, _transferIn, null, null, "family", _user.Id.Value));
    }

    private async Task TheAccountIsReportedMissing(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Detail.Should().StartWith("Account '").And.EndWith("' does not exists.");
        (await server.Database.TransactionsInDbAsync(_chequing)).Should().ContainSingle().Which.Amount.Should().Be(123.45m);
    }

    private static async Task TheTransactionIsNotFound(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Detail.Should().Be(TransactionErrors.NotFound.Description);
    }

    private static async Task AValidationErrorIsReturnedOnTheDate(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem.Errors.Should().ContainKey("date-invalid");
    }
}
