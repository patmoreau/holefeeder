using System.Net;

using FluentAssertions;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.SeedWork;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

using Xunit;

namespace Holefeeder.FunctionalTests.Features.Enumerations;

public class ScenarioGetCategoryTypes : BaseScenario
{
    public ScenarioGetCategoryTypes(ApiApplicationDriver apiApplicationDriver) : base(apiApplicationDriver)
    {
    }

    [Fact]
    public async Task WhenAnonymousUser()
    {
        GivenUserIsUnauthorized();

        await WhenUserGetEnumeration();

        ThenShouldExpectStatusCode(HttpStatusCode.OK);
        var result = HttpClientDriver.DeserializeContent<CategoryType[]>();
        ThenAssertAll(() =>
        {
            result.Should().NotBeNull().And.HaveCount(Enumeration.GetAll<CategoryType>().Count());
        });
    }

    private async Task WhenUserGetEnumeration()
    {
        await HttpClientDriver.SendGetRequest(ApiResources.GetCategoryTypes);
    }
}
