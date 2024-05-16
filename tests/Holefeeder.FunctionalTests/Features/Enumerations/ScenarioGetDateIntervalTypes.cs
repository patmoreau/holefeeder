using System.Net;

using Holefeeder.Domain.Enumerations;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

namespace Holefeeder.FunctionalTests.Features.Enumerations;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetDateIntervalTypes(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper) : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public async Task WhenAnonymousUser()
    {
        GivenUserIsUnauthorized();

        await WhenUserGetEnumeration();

        ShouldExpectStatusCode(HttpStatusCode.OK);
        var result = HttpClientDriver.DeserializeContent<DateIntervalType[]>();
        AssertAll(() => { result.Should().NotBeNull().And.HaveCount(DateIntervalType.List.Count); });
    }

    private async Task WhenUserGetEnumeration() => await HttpClientDriver.SendRequestAsync(ApiResources.GetDateIntervalTypes);
}
