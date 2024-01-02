using System.Net;

using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.StoreItems.StoreItemBuilder;

namespace Holefeeder.FunctionalTests.Features.StoreItems;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetStoreItem(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper) : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public async Task WhenNotFound()
    {
        GivenUserIsAuthorized();

        await WhenUserGetStoreItem(Guid.NewGuid());

        ShouldExpectStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        GivenUserIsAuthorized();

        await WhenUserGetStoreItem(Guid.Empty);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenStoreItemExists()
    {
        var storeItem = await GivenAStoreItem()
            .ForUser(HolefeederUserId)
            .SavedInDbAsync(DatabaseDriver);

        GivenUserIsAuthorized();

        await WhenUserGetStoreItem(storeItem.Id);

        ShouldExpectStatusCode(HttpStatusCode.OK);
        var result = HttpClientDriver.DeserializeContent<StoreItemViewModel>();
        result.Should().NotBeNull().And.BeEquivalentTo(storeItem, options => options.Excluding(x => x.UserId));
    }

    private async Task WhenUserGetStoreItem(Guid id) => await HttpClientDriver.SendGetRequestAsync(ApiResources.GetStoreItem, new object[] { id.ToString() });
}
