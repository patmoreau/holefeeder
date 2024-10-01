using System.Net;
using System.Text.Json;

using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.Tests.Common;

using static Holefeeder.Application.Features.StoreItems.Commands.CreateStoreItem;
using static Holefeeder.Tests.Common.Builders.StoreItems.CreateStoreItemRequestBuilder;
using static Holefeeder.Tests.Common.Builders.StoreItems.StoreItemBuilder;

namespace Holefeeder.FunctionalTests.Features.StoreItems;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioCreateStoreItem(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public async Task WhenInvalidRequest()
    {
        var storeItem = GivenACreateStoreItemRequest()
            .WithCode(string.Empty)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserCreateStoreItem(storeItem);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.", HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenCreateStoreItem()
    {
        var storeItem = GivenACreateStoreItemRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserCreateStoreItem(storeItem);

        ShouldExpectStatusCode(HttpStatusCode.Created);

        ShouldGetTheRouteOfTheNewResourceInTheHeader();
    }

    [Fact]
    public async Task WhenCodeAlreadyExist()
    {
        var existingItem = await GivenAStoreItem()
            .ForUser(TestUsers[AuthorizedUser].UserId)
            .SavedInDbAsync(DatabaseDriver);

        var storeItem = GivenACreateStoreItemRequest()
            .WithCode(existingItem.Code)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserCreateStoreItem(storeItem);

        ShouldExpectStatusCode(HttpStatusCode.BadRequest);
    }

    private async Task WhenUserCreateStoreItem(Request request)
    {
        var json = JsonSerializer.Serialize(request, Globals.JsonSerializerOptions);
        await HttpClientDriver.SendRequestWithBodyAsync(ApiResources.CreateStoreItem, json);
    }
}
