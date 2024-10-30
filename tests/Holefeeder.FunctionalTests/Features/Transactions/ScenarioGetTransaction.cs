// using System.Net;
//
// using Holefeeder.Application.Models;
// using Holefeeder.Domain.Features.Accounts;
// using Holefeeder.Domain.Features.Categories;
// using Holefeeder.FunctionalTests.Drivers;
// using Holefeeder.FunctionalTests.Infrastructure;
//
// using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
// using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
// using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;
//
// namespace Holefeeder.FunctionalTests.Features.Transactions;
//
// [ComponentTest]
// [Collection("Api collection")]
// public class ScenarioGetTransaction(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper) : HolefeederScenario(applicationDriver, testOutputHelper)
// {
//     [Fact]
//     public async Task WhenNotFound()
//     {
//         GivenUserIsAuthorized();
//
//         await WhenUserGetTransaction(Guid.NewGuid());
//
//         ShouldExpectStatusCode(HttpStatusCode.NotFound);
//     }
//
//     [Fact]
//     public async Task WhenInvalidRequest()
//     {
//         GivenUserIsAuthorized();
//
//         await WhenUserGetTransaction(Guid.Empty);
//
//         ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.", HttpStatusCode.BadRequest);
//     }
//
//     [Fact]
//     public async Task WhenTransactionExists()
//     {
//         var account = await GivenAnActiveAccount()
//             .OfType(AccountType.Checking)
//             .ForUser(TestUsers[AuthorizedUser].UserId)
//             .SavedInDbAsync(DatabaseDriver);
//
//         var category = await GivenACategory()
//             .OfType(CategoryType.Expense)
//             .ForUser(TestUsers[AuthorizedUser].UserId)
//             .SavedInDbAsync(DatabaseDriver);
//
//         var transaction = await GivenATransaction()
//             .ForAccount(account)
//             .ForCategory(category)
//             .SavedInDbAsync(DatabaseDriver);
//
//         GivenUserIsAuthorized();
//
//         await WhenUserGetTransaction(transaction.Id);
//
//         ShouldExpectStatusCode(HttpStatusCode.OK);
//         var result = HttpClientDriver.DeserializeContent<TransactionInfoViewModel>();
//         AssertAll(() =>
//         {
//             result.Should()
//                 .NotBeNull()
//                 .And
//                 .BeEquivalentTo(new
//                 {
//                     Id = (Guid)transaction.Id,
//                     transaction.Date,
//                     Amount = (decimal)transaction.Amount,
//                     transaction.Description,
//                     transaction.Tags,
//                     Category = new
//                     {
//                         Id = (Guid)category.Id,
//                         category.Name,
//                         category.Type,
//                         Color = (string)category.Color
//                     },
//                     Account = new
//                     {
//                         Id = (Guid)account.Id,
//                         account.Name
//                     }
//                 });
//         });
//     }
//
//     private async Task WhenUserGetTransaction(Guid id) => await HttpClientDriver.SendRequestAsync(ApiResources.GetTransaction, id);
// }
