using System.Net;
using System.Text.Json;

using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.Infrastructure.Entities;

using Xunit;
using Xunit.Abstractions;

using static Holefeeder.Tests.Common.Builders.StoreItemEntityBuilder;

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
        var storeItem = GivenAStoreItem()
            .WithCode(string.Empty)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserCreateStoreItem(storeItem);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        var storeItem = GivenAStoreItem().Build();

        GivenUserIsAuthorized();

        await WhenUserCreateStoreItem(storeItem);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        var storeItem = GivenAStoreItem().Build();

        GivenForbiddenUserIsAuthorized();

        await WhenUserCreateStoreItem(storeItem);

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        var storeItem = GivenAStoreItem().Build();

        GivenUserIsUnauthorized();

        await WhenUserCreateStoreItem(storeItem);

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenCreateStoreItem()
    {
        var storeItem = GivenAStoreItem().Build();

        GivenUserIsAuthorized();

        await WhenUserCreateStoreItem(storeItem);

        ThenShouldExpectStatusCode(HttpStatusCode.Created);

        ThenShouldGetTheRouteOfTheNewResourceInTheHeader();
    }

    private async Task WhenUserCreateStoreItem(StoreItemEntity entity)
    {
        var json = JsonSerializer.Serialize(new {entity.Code, entity.Data});
        await HttpClientDriver.SendPostRequest(ApiResources.CreateStoreItem, json);
    }
}
