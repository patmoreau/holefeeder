using System.Net;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using static Holefeeder.Application.Features.Transactions.Commands.DeleteTransaction;
using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.DeleteTransactionRequestBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioDeleteTransaction : HolefeederScenario
{
    public ScenarioDeleteTransaction(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, testOutputHelper)
    {
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
            .SavedInDbAsync(DatabaseDriver);

        Category category = await GivenACategory()
            .ForUser(HolefeederUserId)
            .SavedInDbAsync(DatabaseDriver);

        Transaction transaction = await GivenATransaction()
            .ForAccount(account)
            .ForCategory(category)
            .SavedInDbAsync(DatabaseDriver);

        Request request = GivenADeleteTransactionRequest().WithId(transaction.Id).Build();

        GivenUserIsAuthorized();

        await WhenUserDeletesATransaction(request);

        ShouldExpectStatusCode(HttpStatusCode.NoContent);

        using var dbContext = DatabaseDriver.CreateDbContext();

        Transaction? result = await dbContext.FindByIdAsync<Transaction>(transaction.Id);

        result.Should().BeNull();
    }

    private async Task WhenUserDeletesATransaction(Request request) => await HttpClientDriver.SendDeleteRequestAsync(ApiResources.DeleteTransaction, request.Id);
}
