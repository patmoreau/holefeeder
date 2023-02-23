using System.Net;
using System.Text.Json;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.Application.Features.StoreItems.Commands.CreateStoreItem;
using static Holefeeder.Tests.Common.Builders.StoreItems.CreateStoreItemRequestBuilder;
using static Holefeeder.Tests.Common.Builders.StoreItems.StoreItemBuilder;

namespace Holefeeder.FunctionalTests.Features.StoreItems;

public class ScenarioCreateStoreItem : BaseScenario
{
    public ScenarioCreateStoreItem(ApiApplicationDriver apiApplicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        Request storeItem = GivenACreateStoreItemRequest()
            .WithCode(string.Empty)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserCreateStoreItem(storeItem);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        Request storeItem = GivenACreateStoreItemRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserCreateStoreItem(storeItem);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        Request storeItem = GivenACreateStoreItemRequest().Build();

        GivenForbiddenUserIsAuthorized();

        await WhenUserCreateStoreItem(storeItem);

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        Request storeItem = GivenACreateStoreItemRequest().Build();

        GivenUserIsUnauthorized();

        await WhenUserCreateStoreItem(storeItem);

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenCreateStoreItem()
    {
        Request storeItem = GivenACreateStoreItemRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserCreateStoreItem(storeItem);

        ThenShouldExpectStatusCode(HttpStatusCode.Created);

        ThenShouldGetTheRouteOfTheNewResourceInTheHeader();
    }

    [Fact]
    public async Task WhenCodeAlreadyExist()
    {
        StoreItem existingItem = await GivenAStoreItem()
            .SavedInDb(DatabaseDriver);

        Request storeItem = GivenACreateStoreItemRequest()
            .WithCode(existingItem.Code)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserCreateStoreItem(storeItem);

        ThenShouldExpectStatusCode(HttpStatusCode.BadRequest);
    }

    private async Task WhenUserCreateStoreItem(Request request)
    {
        string json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResources.CreateStoreItem, json);
    }
}
