using System.Net;

using Bogus;

using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;

using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public class ScenarioGetTransactions : BaseScenario
{
    public ScenarioGetTransactions(ApiApplicationDriver apiApplicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        GivenUserIsAuthorized();

        await WhenUserTriesToQuery(ApiResources.GetTransactions, -1);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        GivenUserIsAuthorized();

        await WhenUserTriesToQuery(ApiResources.GetTransactions);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        GivenForbiddenUserIsAuthorized();

        await WhenUserTriesToQuery(ApiResources.GetTransactions);

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        GivenUserIsUnauthorized();

        await WhenUserTriesToQuery(ApiResources.GetTransactions);

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenTransactionsExistsSortedByDescriptionDesc()
    {
        var faker = new Faker();
        var count = faker.Random.Int(2, 10);

        var account = await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .SavedInDb(DatabaseDriver);

        var category = await GivenACategory()
            .OfType(CategoryType.Expense)
            .ForUser(AuthorizedUserId)
            .SavedInDb(DatabaseDriver);

        await GivenATransaction()
            .ForAccount(account)
            .ForCategory(category)
            .CollectionSavedInDb(DatabaseDriver, count);

        GivenUserIsAuthorized();

        await WhenUserTriesToQuery(ApiResources.GetTransactions, sorts: "-description");

        ThenShouldExpectStatusCode(HttpStatusCode.OK);
        var result = HttpClientDriver.DeserializeContent<TransactionInfoViewModel[]>();
        result.Should().NotBeNull().And.HaveCount(count).And.BeInDescendingOrder(x => x.Description);
    }
}
