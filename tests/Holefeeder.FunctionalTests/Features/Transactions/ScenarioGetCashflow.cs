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
// using static Holefeeder.Tests.Common.Builders.Transactions.CashflowBuilder;
//
// namespace Holefeeder.FunctionalTests.Features.Transactions;
//
// [ComponentTest]
// [Collection("Api collection")]
// public class ScenarioGetCashflow(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
//     : HolefeederScenario(applicationDriver, testOutputHelper)
// {
//     [Fact]
//     public async Task WhenNotFound()
//     {
//         GivenUserIsAuthorized();
//
//         await WhenUserGetCashflow(Guid.NewGuid());
//
//         ShouldExpectStatusCode(HttpStatusCode.NotFound);
//     }
//
//     [Fact]
//     public async Task WhenInvalidRequest()
//     {
//         GivenUserIsAuthorized();
//
//         await WhenUserGetCashflow(Guid.Empty);
//
//         ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.", HttpStatusCode.BadRequest);
//     }
//
//     [Fact]
//     public async Task WhenCashflowExists()
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
//         var cashflow = await GivenAnActiveCashflow()
//             .ForAccount(account)
//             .ForCategory(category)
//             .ForUser(TestUsers[AuthorizedUser].UserId)
//             .SavedInDbAsync(DatabaseDriver);
//
//         GivenUserIsAuthorized();
//
//         await WhenUserGetCashflow(cashflow.Id);
//
//         ShouldExpectStatusCode(HttpStatusCode.OK);
//         var result = HttpClientDriver.DeserializeContent<CashflowInfoViewModel>();
//         AssertAll(() =>
//         {
//             result.Should()
//                 .NotBeNull()
//                 .And
//                 .BeEquivalentTo(new
//                 {
//                     Id = (Guid)cashflow.Id,
//                     cashflow.EffectiveDate,
//                     Amount = (decimal)cashflow.Amount,
//                     cashflow.IntervalType,
//                     cashflow.Frequency,
//                     cashflow.Recurrence,
//                     cashflow.Description,
//                     cashflow.Inactive,
//                     cashflow.Tags,
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
//     private async Task WhenUserGetCashflow(Guid id) => await HttpClientDriver.SendRequestAsync(ApiResources.GetCashflow, id.ToString());
// }
