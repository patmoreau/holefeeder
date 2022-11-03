using System.Net;
using System.Text.Json;

using Holefeeder.Application.Features.StoreItems.Commands;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;

using Xunit;
using Xunit.Abstractions;

using static Holefeeder.Tests.Common.Builders.StoreItems.CreateStoreItemRequestBuilder;
using static Holefeeder.Tests.Common.Builders.StoreItems.StoreItemBuilder;

namespace Holefeeder.FunctionalTests.Features.StoreItems;

public class ScenarioCreateStoreItem : BaseScenario
{
    private readonly ObjectStoreDatabaseDriver _objectStoreDatabaseDriver;

    public ScenarioCreateStoreItem(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, testOutputHelper)
    {
        _objectStoreDatabaseDriver = apiApplicationDriver.CreateObjectStoreDatabaseDriver();
        _objectStoreDatabaseDriver.ResetStateAsync().Wait();
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        var storeItem = GivenACreateStoreItemRequest()
            .WithCode(string.Empty)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserCreateStoreItem(storeItem);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        var storeItem = GivenACreateStoreItemRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserCreateStoreItem(storeItem);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        var storeItem = GivenACreateStoreItemRequest().Build();

        GivenForbiddenUserIsAuthorized();

        await WhenUserCreateStoreItem(storeItem);

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        var storeItem = GivenACreateStoreItemRequest().Build();

        GivenUserIsUnauthorized();

        await WhenUserCreateStoreItem(storeItem);

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenCreateStoreItem()
    {
        var storeItem = GivenACreateStoreItemRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserCreateStoreItem(storeItem);

        ThenShouldExpectStatusCode(HttpStatusCode.Created);

        ThenShouldGetTheRouteOfTheNewResourceInTheHeader();
    }

    [Fact]
    public async Task WhenCodeAlreadyExist()
    {
        var existingItem = await GivenAStoreItem()
            .SavedInDb(_objectStoreDatabaseDriver);

        var storeItem = GivenACreateStoreItemRequest()
            .WithCode(existingItem.Code)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserCreateStoreItem(storeItem);

        ThenShouldExpectStatusCode(HttpStatusCode.BadRequest);
    }

    private async Task WhenUserCreateStoreItem(CreateStoreItem.Request request)
    {
        var json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResources.CreateStoreItem, json);
    }
}
