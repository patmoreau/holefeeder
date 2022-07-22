using System.Net;
using System.Text.Json;

using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.Infrastructure.Entities;

using Xunit;
using Xunit.Abstractions;

using static Holefeeder.Tests.Common.Builders.StoreItemEntityBuilder;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.StoreItems;

public class ScenarioModifyStoreItem : BaseScenario
{
    private readonly ObjectStoreDatabaseDriver _objectStoreDatabaseDriver;

    public ScenarioModifyStoreItem(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, testOutputHelper)
    {
        _objectStoreDatabaseDriver = apiApplicationDriver.CreateObjectStoreDatabaseDriver();
        _objectStoreDatabaseDriver.ResetStateAsync().Wait();
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        var storeItem = GivenAStoreItem()
            .WithId(Guid.Empty)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserModifyStoreItem(storeItem);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        var storeItem = GivenAStoreItem().Build();

        GivenUserIsAuthorized();

        await WhenUserModifyStoreItem(storeItem);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        var storeItem = GivenAStoreItem().Build();

        GivenForbiddenUserIsAuthorized();

        await WhenUserModifyStoreItem(storeItem);

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        var storeItem = GivenAStoreItem().Build();

        GivenUserIsUnauthorized();

        await WhenUserModifyStoreItem(storeItem);

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenModifyStoreItem()
    {
        var storeItem = await GivenAStoreItem()
            .ForUser(AuthorizedUserId)
            .SavedInDb(_objectStoreDatabaseDriver);

        GivenUserIsAuthorized();

        await WhenUserModifyStoreItem(storeItem);

        ThenShouldExpectStatusCode(HttpStatusCode.NoContent);
    }

    private async Task WhenUserModifyStoreItem(StoreItemEntity entity)
    {
        var json = JsonSerializer.Serialize(new {entity.Id, entity.Data});
        await HttpClientDriver.SendPostRequest(ApiResources.ModifyStoreItem, json);
    }
}
