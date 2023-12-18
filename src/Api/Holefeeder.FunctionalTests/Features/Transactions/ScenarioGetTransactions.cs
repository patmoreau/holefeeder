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
using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetTransactions : HolefeederScenario
{
    public ScenarioGetTransactions(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        GivenUserIsAuthorized();

        await QueryEndpoint(ApiResources.GetTransactions, -1);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenTransactionsExistsSortedByDescriptionDesc()
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

        await GivenATransaction()
            .ForAccount(account)
            .ForCategory(category)
            .CollectionSavedInDbAsync(DatabaseDriver, count);

        GivenUserIsAuthorized();

        await QueryEndpoint(ApiResources.GetTransactions, sorts: "-description");

        ShouldExpectStatusCode(HttpStatusCode.OK);
        TransactionInfoViewModel[]? result = HttpClientDriver.DeserializeContent<TransactionInfoViewModel[]>();
        result.Should().NotBeNull().And.HaveCount(count).And.BeInDescendingOrder(x => x.Description);
    }
}
