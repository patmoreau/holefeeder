using System.Net;

using Bogus;

using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;

namespace Holefeeder.FunctionalTests.Features.Accounts;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetAccounts(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public async Task WhenInvalidRequest()
    {
        GivenUserIsAuthorized();

        await QueryEndpoint(ApiResources.GetAccounts, -1);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAccountsExistsSortedByNameDesc()
    {
        var faker = new Faker();
        var count = faker.Random.Int(2, 10);

        await GivenAnActiveAccount()
            .ForUser(HolefeederUserId)
            .CollectionSavedInDbAsync(DatabaseDriver, count);

        await GivenAnActiveAccount()
            .CollectionSavedInDbAsync(DatabaseDriver, count);

        GivenUserIsAuthorized();

        await QueryEndpoint(ApiResources.GetAccounts, sorts: "-name");

        ShouldExpectStatusCode(HttpStatusCode.OK);
        AccountViewModel[]? result = HttpClientDriver.DeserializeContent<AccountViewModel[]>();
        result.Should().NotBeNull().And.HaveCount(count).And.BeInDescendingOrder(x => x.Name);
    }
}
