using System.Net;

using FluentAssertions;

using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.SeedWork;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

using Xunit;
using Xunit.Abstractions;

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