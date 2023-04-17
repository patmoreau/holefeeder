using System.Net;
using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.CashflowBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public class ScenarioGetCashflow : BaseScenario
{
    public ScenarioGetCashflow(ApiApplicationDriver applicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenNotFound()
    {
        GivenUserIsAuthorized();

        await WhenUserGetCashflow(Guid.NewGuid());

        ThenShouldExpectStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        GivenUserIsAuthorized();

        await WhenUserGetCashflow(Guid.Empty);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenCashflowExists()
    {
        Account account = await GivenAnActiveAccount()
            .OfType(AccountType.Checking)
            .ForUser(HolefeederUserId)
            .SavedInDb(DatabaseDriver);

        Category category = await GivenACategory()
            .OfType(CategoryType.Expense)
            .ForUser(HolefeederUserId)
            .SavedInDb(DatabaseDriver);

        Cashflow cashflow = await GivenAnActiveCashflow()
            .ForAccount(account)
            .ForCategory(category)
            .ForUser(HolefeederUserId)
            .SavedInDb(DatabaseDriver);

        GivenUserIsAuthorized();

        await WhenUserGetCashflow(cashflow.Id);

        ThenShouldExpectStatusCode(HttpStatusCode.OK);
        CashflowInfoViewModel? result = HttpClientDriver.DeserializeContent<CashflowInfoViewModel>();
        ThenAssertAll(() =>
        {
            result.Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(cashflow, options => options.ExcludingMissingMembers());
        });
    }

    private async Task WhenUserGetCashflow(Guid id) => await HttpClientDriver.SendGetRequest(ApiResources.GetCashflow, new object[] { id.ToString() });
}
