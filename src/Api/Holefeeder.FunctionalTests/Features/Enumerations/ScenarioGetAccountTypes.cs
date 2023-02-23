using System.Net;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

namespace Holefeeder.FunctionalTests.Features.Enumerations;

public class ScenarioGetAccountTypes : BaseScenario
{
    public ScenarioGetAccountTypes(ApiApplicationDriver apiApplicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenAnonymousUser()
    {
        GivenUserIsUnauthorized();

        await WhenUserGetEnumeration();

        ThenShouldExpectStatusCode(HttpStatusCode.OK);
        AccountType[]? result = HttpClientDriver.DeserializeContent<AccountType[]>();
        ThenAssertAll(() => { result.Should().NotBeNull().And.HaveCount(AccountType.List.Count); });
    }

    private async Task WhenUserGetEnumeration() => await HttpClientDriver.SendGetRequest(ApiResources.GetAccountTypes);
}
