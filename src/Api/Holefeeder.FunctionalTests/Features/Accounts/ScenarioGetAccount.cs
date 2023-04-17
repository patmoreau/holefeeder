using System.Net;
using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;

namespace Holefeeder.FunctionalTests.Features.Accounts;

public class ScenarioGetAccount : BaseScenario
{
    public ScenarioGetAccount(ApiApplicationDriver applicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenNotFound()
    {
        GivenUserIsAuthorized();

        await WhenUserGetAccount(Guid.NewGuid());

        ThenShouldExpectStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        GivenUserIsAuthorized();

        await WhenUserGetAccount(Guid.Empty);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAccountExistsWithExpenses()
    {
        Account account = await GivenAnActiveAccount()
            .OfType(AccountType.Checking)
            .ForUser(HolefeederUserId)
            .SavedInDb(DatabaseDriver);

        Category category = await GivenACategory()
            .OfType(CategoryType.Expense)
            .ForUser(HolefeederUserId)
            .SavedInDb(DatabaseDriver);

        Transaction transaction = await GivenATransaction()
            .ForAccount(account)
            .ForCategory(category)
            .SavedInDb(DatabaseDriver);

        GivenUserIsAuthorized();

        await WhenUserGetAccount(account.Id);

        ThenShouldExpectStatusCode(HttpStatusCode.OK);
        AccountViewModel? result = HttpClientDriver.DeserializeContent<AccountViewModel>();
        ThenAssertAll(() =>
        {
            result.Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(account, options => options.ExcludingMissingMembers());
            result!.TransactionCount.Should().Be(1);
            result.Balance.Should().Be(account.OpenBalance - transaction.Amount);
        });
    }

    [Fact]
    public async Task WhenAccountExistsWithGains()
    {
        Account account = await GivenAnActiveAccount()
            .OfType(AccountType.Checking)
            .ForUser(HolefeederUserId)
            .SavedInDb(DatabaseDriver);

        Category category = await GivenACategory()
            .OfType(CategoryType.Gain)
            .ForUser(HolefeederUserId)
            .SavedInDb(DatabaseDriver);

        Transaction transaction = await GivenATransaction()
            .ForAccount(account)
            .ForCategory(category)
            .SavedInDb(DatabaseDriver);

        GivenUserIsAuthorized();

        await WhenUserGetAccount(account.Id);

        ThenShouldExpectStatusCode(HttpStatusCode.OK);
        AccountViewModel? result = HttpClientDriver.DeserializeContent<AccountViewModel>();
        ThenAssertAll(() =>
        {
            result.Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(account, options => options.ExcludingMissingMembers());
            result!.TransactionCount.Should().Be(1);
            result.Balance.Should().Be(account.OpenBalance + transaction.Amount);
        });
    }

    private async Task WhenUserGetAccount(Guid id) => await HttpClientDriver.SendGetRequest(ApiResources.GetAccount, new object[] { id.ToString() });
}
