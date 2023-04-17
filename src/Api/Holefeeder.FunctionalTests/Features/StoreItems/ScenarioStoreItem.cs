using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.StepDefinitions;
using CreateRequest = Holefeeder.Application.Features.StoreItems.Commands.CreateStoreItem.Request;
using ModifyRequest = Holefeeder.Application.Features.StoreItems.Commands.ModifyStoreItem.Request;

namespace Holefeeder.FunctionalTests.Features.StoreItems;

[Collection("Api collection")]
public class ScenarioStoreItem : BaseScenario
{
    private readonly StoreItemStepDefinition _storeItem;

    public ScenarioStoreItem(ApiApplicationDriver applicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
        _storeItem = new StoreItemStepDefinition(HttpClientDriver);
    }

    [Fact]
    public Task UserCreatesStoreItem() =>
        ScenarioFor("user creating a store item", runner =>
            runner.Given(User.IsAuthorized)
                .When(_storeItem.GetsCreated)
                .Then(_storeItem.ShouldMatchTheCreationRequest));

    [Fact]
    public Task UserModifiesStoreItem() =>
        ScenarioFor("user modifying a store item", runner =>
            runner.Given(User.IsAuthorized)
                .And(_storeItem.Exists)
                .When(_storeItem.GetsModified)
                .Then(_storeItem.ShouldMatchTheModificationRequest));
}
