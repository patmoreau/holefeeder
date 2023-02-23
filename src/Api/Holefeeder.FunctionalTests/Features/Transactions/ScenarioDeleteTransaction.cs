using System.Net;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.Application.Features.Transactions.Commands.DeleteTransaction;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.DeleteTransactionRequestBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public class ScenarioDeleteTransaction : BaseScenario
{
    public ScenarioDeleteTransaction(ApiApplicationDriver apiApplicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
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
        Request request = GivenAnInvalidDeleteTransactionRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserDeletesATransaction(request);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        Request request = GivenADeleteTransactionRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserDeletesATransaction(request);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        Request request = GivenADeleteTransactionRequest().Build();

        GivenForbiddenUserIsAuthorized();

        await WhenUserDeletesATransaction(request);

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        Request request = GivenADeleteTransactionRequest().Build();

        GivenUserIsUnauthorized();

        await WhenUserDeletesATransaction(request);

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenDeletingATransaction()
    {
        Account account = await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .SavedInDb(DatabaseDriver);

        Category category = await GivenACategory()
            .ForUser(AuthorizedUserId)
            .SavedInDb(DatabaseDriver);

        Transaction transaction = await GivenATransaction()
            .ForAccount(account)
            .ForCategory(category)
            .SavedInDb(DatabaseDriver);

        Request request = GivenADeleteTransactionRequest().WithId(transaction.Id).Build();

        GivenUserIsAuthorized();

        await WhenUserDeletesATransaction(request);

        ThenShouldExpectStatusCode(HttpStatusCode.NoContent);

        Transaction? result = await DatabaseDriver.FindByIdAsync<Transaction>(transaction.Id);

        result.Should().BeNull();
    }

    private async Task WhenUserDeletesATransaction(Request request) => await HttpClientDriver.SendDeleteRequest(ApiResources.DeleteTransaction, request.Id);
}
