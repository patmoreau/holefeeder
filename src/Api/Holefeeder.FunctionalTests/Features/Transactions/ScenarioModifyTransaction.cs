using System.Net;
using System.Text.Json;

using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;

using static Holefeeder.Application.Features.Transactions.Commands.ModifyTransaction;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.ModifyTransactionRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public class ScenarioModifyTransaction : BaseScenario
{
    public ScenarioModifyTransaction(ApiApplicationDriver apiApplicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
        if (apiApplicationDriver == null)
        {
            throw new ArgumentNullException(nameof(apiApplicationDriver));
        }
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        var request = GivenAnInvalidModifyTransactionRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserModifiedATransaction(request);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        var request = GivenAModifyTransactionRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserModifiedATransaction(request);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        var request = GivenAModifyTransactionRequest().Build();

        GivenForbiddenUserIsAuthorized();

        await WhenUserModifiedATransaction(request);

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        var request = GivenAModifyTransactionRequest().Build();

        GivenUserIsUnauthorized();

        await WhenUserModifiedATransaction(request);

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenAccountDoesNotExists()
    {
        var request = GivenAModifyTransactionRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserModifiedATransaction(request);

        ThenShouldReceiveProblemDetailsWithErrorMessage(HttpStatusCode.BadRequest,
            $"Account '{request.AccountId}' does not exists.");
    }

    [Fact]
    public async Task WhenCategoryDoesNotExists()
    {
        var account = await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .SavedInDb(DatabaseDriver);

        var request = GivenAModifyTransactionRequest()
            .WithAccount(account).Build();

        GivenUserIsAuthorized();

        await WhenUserModifiedATransaction(request);

        ThenShouldReceiveProblemDetailsWithErrorMessage(HttpStatusCode.BadRequest,
            $"Category '{request.CategoryId}' does not exists.");
    }

    [Fact]
    public async Task WhenTransactionDoesNotExists()
    {
        var account = await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .SavedInDb(DatabaseDriver);

        var category = await GivenACategory()
            .ForUser(AuthorizedUserId)
            .SavedInDb(DatabaseDriver);

        var request = GivenAModifyTransactionRequest()
            .WithAccount(account)
            .WithCategory(category)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserModifiedATransaction(request);

        ThenShouldReceiveProblemDetailsWithErrorMessage(HttpStatusCode.NotFound,
            $"'{request.Id}' not found");
    }

    [Fact]
    public async Task WhenModifyATransaction()
    {
        var accounts = await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .CollectionSavedInDb(DatabaseDriver, 2);

        var categories = await GivenACategory()
            .ForUser(AuthorizedUserId)
            .CollectionSavedInDb(DatabaseDriver, 2);

        var transaction = await GivenATransaction()
            .ForAccount(accounts[0])
            .ForCategory(categories[0])
            .SavedInDb(DatabaseDriver);

        GivenUserIsAuthorized();

        var request = GivenAModifyTransactionRequest()
            .WithId(transaction.Id)
            .WithAccount(accounts[1])
            .WithCategory(categories[1])
            .Build();

        await WhenUserModifiedATransaction(request);

        ThenShouldExpectStatusCode(HttpStatusCode.NoContent);

        var result = await DatabaseDriver.FindByIdAsync<Transaction>(transaction.Id);

        result.Should().NotBeNull().And.BeEquivalentTo(request, options => options.ExcludingMissingMembers());
    }

    private async Task WhenUserModifiedATransaction(Request request)
    {
        var json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResources.ModifyTransaction, json);
    }
}
