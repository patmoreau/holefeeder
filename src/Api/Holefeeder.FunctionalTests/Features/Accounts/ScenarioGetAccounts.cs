using System.Net;
using Bogus;
using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;

namespace Holefeeder.FunctionalTests.Features.Accounts;

public class ScenarioGetAccounts : BaseScenario
{
    public ScenarioGetAccounts(ApiApplicationDriver apiApplicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        GivenUserIsAuthorized();

        await WhenUserTriesToQuery(ApiResource.GetAccounts, -1);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAccountsExistsSortedByNameDesc()
    {
        Faker faker = new Faker();
        int count = faker.Random.Int(2, 10);

        await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .CollectionSavedInDb(DatabaseDriver, count);

        await GivenAnActiveAccount()
            .CollectionSavedInDb(DatabaseDriver, count);

        GivenUserIsAuthorized();

        await WhenUserTriesToQuery(ApiResource.GetAccounts, sorts: "-name");

        ThenShouldExpectStatusCode(HttpStatusCode.OK);
        AccountViewModel[]? result = HttpClientDriver.DeserializeContent<AccountViewModel[]>();
        result.Should().NotBeNull().And.HaveCount(count).And.BeInDescendingOrder(x => x.Name);
    }
}
