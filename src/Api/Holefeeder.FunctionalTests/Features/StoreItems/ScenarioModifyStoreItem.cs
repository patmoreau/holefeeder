using System.Net;
using System.Text.Json;

using FluentAssertions;

using Holefeeder.Application.Features.StoreItems.Commands.ModifyStoreItem;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;

using static Holefeeder.Tests.Common.Builders.StoreItems.ModifyStoreItemRequestBuilder;
using static Holefeeder.Tests.Common.Builders.StoreItems.StoreItemBuilder;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.StoreItems;

public class ScenarioModifyStoreItem : BaseScenario
{
    private readonly BudgetingDatabaseDriver _databaseDriver;

    public ScenarioModifyStoreItem(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, testOutputHelper)
    {
        _databaseDriver = BudgetingDatabaseDriver;
        _databaseDriver.ResetStateAsync().Wait();
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
            .SavedInDb(_databaseDriver);

        var request = GivenAModifyStoreItemRequest()
            .WithId(storeItem.Id)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserModifyStoreItem(request);

        ThenShouldExpectStatusCode(HttpStatusCode.NoContent);

        var result = await _databaseDriver.FindByIdAsync<StoreItem>(storeItem.Id);
        result.Should().NotBeNull();
        result!.Data.Should().BeEquivalentTo(request.Data);

    }

    private async Task WhenUserModifyStoreItem(Request request)
    {
        var json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResources.ModifyStoreItem, json);
    }
}
