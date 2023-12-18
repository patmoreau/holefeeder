using System.Net;
using System.Text.Json;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using static Holefeeder.Application.Features.StoreItems.Commands.ModifyStoreItem;
using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.StoreItems.ModifyStoreItemRequestBuilder;
using static Holefeeder.Tests.Common.Builders.StoreItems.StoreItemBuilder;

namespace Holefeeder.FunctionalTests.Features.StoreItems;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioModifyStoreItem : HolefeederScenario
{
    public ScenarioModifyStoreItem(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        Request storeItem = GivenAModifyStoreItemRequest()
            .WithId(Guid.Empty)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserModifyStoreItem(storeItem);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenModifyStoreItem()
    {
        StoreItem storeItem = await GivenAStoreItem()
            .ForUser(HolefeederUserId)
            .SavedInDbAsync(DatabaseDriver);

        Request request = GivenAModifyStoreItemRequest()
            .WithId(storeItem.Id)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserModifyStoreItem(request);

        ShouldExpectStatusCode(HttpStatusCode.NoContent);

        using var dbContext = DatabaseDriver.CreateDbContext();

        StoreItem? result = await dbContext.FindByIdAsync<StoreItem>(storeItem.Id);
        result.Should().NotBeNull();
        result!.Data.Should().BeEquivalentTo(request.Data);
    }

    private async Task WhenUserModifyStoreItem(Request request)
    {
        string json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequestAsync(ApiResources.ModifyStoreItem, json);
    }
}
