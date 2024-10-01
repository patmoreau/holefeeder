using System.Net;

using Bogus;

using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.CashflowBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetCashflows(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public async Task WhenInvalidRequest()
    {
        GivenUserIsAuthorized();

        await QueryEndpoint(ApiResources.GetCashflows, -1);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.", HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenCashflowsExistsSortedByDescriptionDesc()
    {
        var faker = new Faker();
        var count = faker.Random.Int(2, 10);

        var account = await GivenAnActiveAccount()
            .ForUser(TestUsers[AuthorizedUser].UserId)
            .SavedInDbAsync(DatabaseDriver);

        var category = await GivenACategory()
            .OfType(CategoryType.Expense)
            .ForUser(TestUsers[AuthorizedUser].UserId)
            .SavedInDbAsync(DatabaseDriver);

        await GivenAnActiveCashflow()
            .ForAccount(account)
            .ForCategory(category)
            .ForUser(TestUsers[AuthorizedUser].UserId)
            .CollectionSavedInDbAsync(DatabaseDriver, count);

        GivenUserIsAuthorized();

        await QueryEndpoint(ApiResources.GetCashflows, sorts: "-description");

        ShouldExpectStatusCode(HttpStatusCode.OK);
        var result = HttpClientDriver.DeserializeContent<CashflowInfoViewModel[]>();
        result.Should().NotBeNull().And.HaveCount(count).And.BeInDescendingOrder(x => x.Description);
    }
}
