using System.Net;
using System.Text.Json;

using Holefeeder.Application.Features.StoreItems.Commands;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;

using Xunit;
using Xunit.Abstractions;

using static Holefeeder.Tests.Common.Builders.StoreItems.ModifyStoreItemRequestBuilder;
using static Holefeeder.Tests.Common.Builders.StoreItems.StoreItemBuilder;
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
        var storeItem = GivenAModifyStoreItemRequest()
            .WithId(Guid.Empty)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserModifyStoreItem(storeItem);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        var storeItem = GivenAModifyStoreItemRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserModifyStoreItem(storeItem);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        var storeItem = GivenAModifyStoreItemRequest().Build();

        GivenForbiddenUserIsAuthorized();

        await WhenUserModifyStoreItem(storeItem);

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        var storeItem = GivenAModifyStoreItemRequest().Build();

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

        var request = GivenAModifyStoreItemRequest()
            .WithId(storeItem.Id)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserModifyStoreItem(request);

        ThenShouldExpectStatusCode(HttpStatusCode.NoContent);
    }

    private async Task WhenUserModifyStoreItem(ModifyStoreItem.Request request)
    {
        var json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResources.ModifyStoreItem, json);
    }
}
