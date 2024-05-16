using System.Net;

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
public class ScenarioDeleteTransaction(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public async Task WhenInvalidRequest()
    {
        var request = GivenAnInvalidDeleteTransactionRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserDeletesATransaction(request);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenDeletingATransaction()
    {
        var account = await GivenAnActiveAccount()
            .ForUser(HolefeederUserId)
            .SavedInDbAsync(DatabaseDriver);

        var category = await GivenACategory()
            .ForUser(HolefeederUserId)
            .SavedInDbAsync(DatabaseDriver);

        var transaction = await GivenATransaction()
            .ForAccount(account)
            .ForCategory(category)
            .SavedInDbAsync(DatabaseDriver);

        var request = GivenADeleteTransactionRequest().WithId(transaction.Id).Build();

        GivenUserIsAuthorized();

        await WhenUserDeletesATransaction(request);

        ShouldExpectStatusCode(HttpStatusCode.NoContent);

        await using var dbContext = DatabaseDriver.CreateDbContext();

        var result = await dbContext.FindByIdAsync<Transaction>(transaction.Id);

        result.Should().BeNull();
    }

    private async Task WhenUserDeletesATransaction(Request request) => await HttpClientDriver.SendRequestAsync(ApiResources.DeleteTransaction, request.Id);
}
