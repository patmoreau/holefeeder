using System.Net;
using Bogus;
using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.CashflowBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

[ComponentTest]
public class ScenarioGetCashflows : HolefeederScenario
{
    public ScenarioGetCashflows(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        GivenUserIsAuthorized();

        await QueryEndpoint(ApiResources.GetCashflows, -1);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenCashflowsExistsSortedByDescriptionDesc()
    {
        Faker faker = new Faker();
        int count = faker.Random.Int(2, 10);

        Account account = await GivenAnActiveAccount()
            .ForUser(HolefeederUserId)
            .SavedInDbAsync(DatabaseDriver);

        Category category = await GivenACategory()
            .OfType(CategoryType.Expense)
            .ForUser(HolefeederUserId)
            .SavedInDbAsync(DatabaseDriver);

        await GivenAnActiveCashflow()
            .ForAccount(account)
            .ForCategory(category)
            .ForUser(HolefeederUserId)
            .CollectionSavedInDbAsync(DatabaseDriver, count);

        GivenUserIsAuthorized();

        await QueryEndpoint(ApiResources.GetCashflows, sorts: "-description");

        ShouldExpectStatusCode(HttpStatusCode.OK);
        CashflowInfoViewModel[]? result = HttpClientDriver.DeserializeContent<CashflowInfoViewModel[]>();
        result.Should().NotBeNull().And.HaveCount(count).And.BeInDescendingOrder(x => x.Description);
    }
}
