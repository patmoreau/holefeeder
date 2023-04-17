using System.Net;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.Application.Features.Transactions.Commands.DeleteTransaction;
using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.DeleteTransactionRequestBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public class ScenarioDeleteTransaction : BaseScenario
{
    public ScenarioDeleteTransaction(ApiApplicationDriver applicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
        if (applicationDriver == null)
        {
            throw new ArgumentNullException(nameof(applicationDriver));
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
    public async Task WhenDeletingATransaction()
    {
        Account account = await GivenAnActiveAccount()
            .ForUser(HolefeederUserId)
            .SavedInDb(DatabaseDriver);

        Category category = await GivenACategory()
            .ForUser(HolefeederUserId)
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
