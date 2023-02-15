using System.Net;

using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;

using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public class ScenarioGetTransaction : BaseScenario
{
    public ScenarioGetTransaction(ApiApplicationDriver apiApplicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenNotFound()
    {
        GivenUserIsAuthorized();

        await WhenUserGetTransaction(Guid.NewGuid());

        ThenShouldExpectStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        GivenUserIsAuthorized();

        await WhenUserGetTransaction(Guid.Empty);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        GivenUserIsAuthorized();

        await WhenUserGetTransaction(Guid.NewGuid());

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        GivenForbiddenUserIsAuthorized();

        await WhenUserGetTransaction(Guid.NewGuid());

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        GivenUserIsUnauthorized();

        await WhenUserGetTransaction(Guid.NewGuid());

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenTransactionExists()
    {
        var account = await GivenAnActiveAccount()
            .OfType(AccountType.Checking)
            .ForUser(AuthorizedUserId)
            .SavedInDb(DatabaseDriver);

        var category = await GivenACategory()
            .OfType(CategoryType.Expense)
            .ForUser(AuthorizedUserId)
            .SavedInDb(DatabaseDriver);

        var transaction = await GivenATransaction()
            .ForAccount(account)
            .ForCategory(category)
            .SavedInDb(DatabaseDriver);

        GivenUserIsAuthorized();

        await WhenUserGetTransaction(transaction.Id);

        ThenShouldExpectStatusCode(HttpStatusCode.OK);
        var result = HttpClientDriver.DeserializeContent<TransactionInfoViewModel>();
        ThenAssertAll(() =>
        {
            result.Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(transaction, options => options.ExcludingMissingMembers());
        });
    }

    private async Task WhenUserGetTransaction(Guid id)
    {
        await HttpClientDriver.SendGetRequest(ApiResources.GetTransaction, new object?[] {id.ToString()});
    }
}
