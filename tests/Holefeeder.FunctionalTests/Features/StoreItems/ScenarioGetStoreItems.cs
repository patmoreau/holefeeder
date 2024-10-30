// using System.Net;
//
// using Holefeeder.Application.Features.StoreItems.Queries;
// using Holefeeder.FunctionalTests.Drivers;
// using Holefeeder.FunctionalTests.Infrastructure;
//
// using static Holefeeder.Tests.Common.Builders.StoreItems.StoreItemBuilder;
//
// namespace Holefeeder.FunctionalTests.Features.StoreItems;
//
// [ComponentTest]
// [Collection("Api collection")]
// public class ScenarioGetStoreItems(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
//     : HolefeederScenario(applicationDriver, testOutputHelper)
// {
//     [Fact]
//     public async Task WhenInvalidRequest()
//     {
//         GivenUserIsAuthorized();
//
//         await QueryEndpoint(ApiResources.GetStoreItems, -1);
//
//         ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.", HttpStatusCode.BadRequest);
//     }
//
//     [Fact]
//     public async Task WhenAccountsExists()
//     {
//         const string firstCode = nameof(firstCode);
//         const string secondCode = nameof(secondCode);
//
//         await GivenAStoreItem()
//             .ForUser(TestUsers[AuthorizedUser].UserId)
//             .WithCode(firstCode)
//             .SavedInDbAsync(DatabaseDriver);
//
//         await GivenAStoreItem()
//             .ForUser(TestUsers[AuthorizedUser].UserId)
//             .WithCode(secondCode)
//             .SavedInDbAsync(DatabaseDriver);
//
//         GivenUserIsAuthorized();
//
//         await QueryEndpoint(ApiResources.GetStoreItems, sorts: "-code");
//
//         ShouldExpectStatusCode(HttpStatusCode.OK);
//         var result = HttpClientDriver.DeserializeContent<StoreItemViewModel[]>();
//         AssertAll(() =>
//         {
//             result.Should().NotBeNull().And.HaveCount(2);
//             result![0].Code.Should().Be(secondCode);
//             result[1].Code.Should().Be(firstCode);
//         });
//     }
// }
