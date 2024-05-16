using System.Net;
using System.Text.Json;

using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

using Microsoft.EntityFrameworkCore;

using static Holefeeder.Application.Features.Transactions.Commands.ModifyTransaction;
using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.ModifyTransactionRequestBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioModifyTransaction(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper) : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public async Task WhenInvalidRequest()
    {
        var request = GivenAnInvalidModifyTransactionRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserModifiedATransaction(request);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAccountDoesNotExists()
    {
        var request = GivenAModifyTransactionRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserModifiedATransaction(request);

        ShouldReceiveProblemDetailsWithErrorMessage(HttpStatusCode.BadRequest,
            $"Account '{request.AccountId}' does not exists.");
    }

    [Fact]
    public async Task WhenCategoryDoesNotExists()
    {
        var account = await GivenAnActiveAccount()
            .ForUser(HolefeederUserId)
            .SavedInDbAsync(DatabaseDriver);

        var request = GivenAModifyTransactionRequest()
            .WithAccount(account).Build();

        GivenUserIsAuthorized();

        await WhenUserModifiedATransaction(request);

        ShouldReceiveProblemDetailsWithErrorMessage(HttpStatusCode.BadRequest,
            $"Category '{request.CategoryId}' does not exists.");
    }

    [Fact]
    public async Task WhenTransactionDoesNotExists()
    {
        var account = await GivenAnActiveAccount()
            .ForUser(HolefeederUserId)
            .SavedInDbAsync(DatabaseDriver);

        var category = await GivenACategory()
            .ForUser(HolefeederUserId)
            .SavedInDbAsync(DatabaseDriver);

        var request = GivenAModifyTransactionRequest()
            .WithAccount(account)
            .WithCategory(category)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserModifiedATransaction(request);

        ShouldReceiveProblemDetailsWithErrorMessage(HttpStatusCode.NotFound,
            $"'{request.Id}' not found");
    }

    [Fact]
    public async Task WhenModifyATransaction()
    {
        var accounts = await GivenAnActiveAccount()
            .ForUser(HolefeederUserId)
            .CollectionSavedInDbAsync(DatabaseDriver, 2);

        var categories = await GivenACategory()
            .ForUser(HolefeederUserId)
            .CollectionSavedInDbAsync(DatabaseDriver, 2);

        var transaction = await GivenATransaction()
            .ForAccount(accounts.ElementAt(0))
            .ForCategory(categories.ElementAt(0))
            .SavedInDbAsync(DatabaseDriver);

        GivenUserIsAuthorized();

        var request = GivenAModifyTransactionRequest()
            .WithId(transaction.Id)
            .WithAccount(accounts.ElementAt(1))
            .WithCategory(categories.ElementAt(1))
            .Build();

        await WhenUserModifiedATransaction(request);

        ShouldExpectStatusCode(HttpStatusCode.NoContent);

        await using var dbContext = DatabaseDriver.CreateDbContext();

        var result = await dbContext.FindByIdAsync<Transaction>(transaction.Id);

        result.Should().NotBeNull().And.BeEquivalentTo(request);
    }

    private async Task WhenUserModifiedATransaction(Request request)
    {
        var json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendRequestWithBodyAsync(ApiResources.ModifyTransaction, json);
    }
}
