using System.Net;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

namespace Holefeeder.FunctionalTests.Features.Enumerations;

[ComponentTest]
public class ScenarioGetAccountTypes : HolefeederScenario
{
    public ScenarioGetAccountTypes(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenAnonymousUser()
    {
        GivenUserIsUnauthorized();

        await WhenUserGetEnumeration();

        ShouldExpectStatusCode(HttpStatusCode.OK);
        AccountType[]? result = HttpClientDriver.DeserializeContent<AccountType[]>();
        AssertAll(() => { result.Should().NotBeNull().And.HaveCount(AccountType.List.Count); });
    }

    private async Task WhenUserGetEnumeration() => await HttpClientDriver.SendGetRequestAsync(ApiResources.GetAccountTypes);
}
