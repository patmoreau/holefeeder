using System.Net;

using Holefeeder.Domain.Features.Categories;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

namespace Holefeeder.FunctionalTests.Features.Enumerations;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetCategoryTypes(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper) : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public async Task WhenAnonymousUser()
    {
        GivenUserIsUnauthorized();

        await WhenUserGetEnumeration();

        ShouldExpectStatusCode(HttpStatusCode.OK);
        var result = HttpClientDriver.DeserializeContent<CategoryType[]>();
        AssertAll(() => { result.Should().NotBeNull().And.HaveCount(CategoryType.List.Count); });
    }

    private async Task WhenUserGetEnumeration() => await HttpClientDriver.SendRequestAsync(ApiResources.GetCategoryTypes);
}
