using System.Net;

using FluentAssertions;

using Holefeeder.Domain.Enumerations;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

namespace Holefeeder.FunctionalTests.Features.Enumerations;

public class ScenarioGetDateIntervalTypes : BaseScenario
{
    public ScenarioGetDateIntervalTypes(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenAnonymousUser()
    {
        GivenUserIsUnauthorized();

        await WhenUserGetEnumeration();

        ThenShouldExpectStatusCode(HttpStatusCode.OK);
        var result = HttpClientDriver.DeserializeContent<DateIntervalType[]>();
        ThenAssertAll(() =>
        {
            result.Should().NotBeNull().And.HaveCount(DateIntervalType.List.Count);
        });
    }

    private async Task WhenUserGetEnumeration()
    {
        await HttpClientDriver.SendGetRequest(ApiResources.GetDateIntervalTypes);
    }
}
