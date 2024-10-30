// using Holefeeder.FunctionalTests.Drivers;
// using Holefeeder.FunctionalTests.StepDefinitions;
//
// namespace Holefeeder.FunctionalTests.Features.StoreItems;
//
// [ComponentTest]
// [Collection("Api collection")]
// public class ScenarioStoreItem : HolefeederScenario
// {
//     private readonly StoreItemStepDefinition _storeItem;
//
//     public ScenarioStoreItem(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
//         : base(applicationDriver, testOutputHelper)
//     {
//         _storeItem = new StoreItemStepDefinition(HttpClientDriver);
//     }
//
//     [Fact]
//     public Task UserCreatesStoreItem() =>
//         ScenarioFor("user creating a store item", runner =>
//             runner.Given(User.IsAuthorized)
//                 .When(_storeItem.GetsCreated)
//                 .Then(_storeItem.ShouldMatchTheCreationRequest));
//
//     [Fact]
//     public Task UserModifiesStoreItem() =>
//         ScenarioFor("user modifying a store item", runner =>
//             runner.Given(User.IsAuthorized)
//                 .And(_storeItem.Exists)
//                 .When(_storeItem.GetsModified)
//                 .Then(_storeItem.ShouldMatchTheModificationRequest));
// }
