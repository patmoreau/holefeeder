using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.FunctionalTests.Drivers;

using static Holefeeder.Tests.Common.Builders.StoreItems.CreateStoreItemRequestBuilder;
using static Holefeeder.Tests.Common.Builders.StoreItems.ModifyStoreItemRequestBuilder;

using CreateRequest = Holefeeder.Application.Features.StoreItems.Commands.CreateStoreItem.Request;
using ModifyRequest = Holefeeder.Application.Features.StoreItems.Commands.ModifyStoreItem.Request;

namespace Holefeeder.FunctionalTests.Features.StoreItems;

public class ScenarioStoreItem : BaseScenario
{
    public ScenarioStoreItem(ApiApplicationDriver apiApplicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
        if (apiApplicationDriver == null)
        {
            throw new ArgumentNullException(nameof(apiApplicationDriver));
        }
    }

    [Fact]
    public async Task UserCreatesStoreItem()
    {
        Guid id = Guid.Empty;
        CreateRequest createRequest = null!;
        StoreItemViewModel storeItem = null!;

        await ScenarioFor("user creating a store item", player =>
        {
            player
                .Given("the user is authorized", () => User.IsAuthorized())
                .And("they created a valid request", () => createRequest = GivenACreateStoreItemRequest().Build())
                .When("the request is sent", () => id = StoreItem.GetsCreated(createRequest).WithCreatedId())
                .And("the store item is retrieved using the id", () => storeItem = StoreItem.RetrievedById(id).WithResultAs<StoreItemViewModel>())
                .Then("the stored item should match the request", () =>
                {
                    storeItem.Id.Should().Be(id);
                    storeItem.Should().BeEquivalentTo(createRequest);
                });
        });
    }

    [Fact]
    public async Task UserModifiesStoreItem()
    {
        Guid id = Guid.Empty;
        ModifyRequest request = null!;
        StoreItemViewModel storeItem = null!;

        await ScenarioFor("modifying a store item", player =>
        {
            player
                .Given("the user is authorized", () => User.IsAuthorized())
                .And("a store item is already created", () => id = StoreItem.GetsCreated().WithCreatedId())
                .And("a valid request is built", () => request = GivenAModifyStoreItemRequest().WithId(id).Build())
                .When("the request is sent", () => storeItem = StoreItem.GetsModified().RetrievedById(id).WithResultAs<StoreItemViewModel>())
                .Then("the data should be modified", () => storeItem.Should().BeEquivalentTo(request));
        });
    }
}
