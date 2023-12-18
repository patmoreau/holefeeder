using System.Net;
using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.CashflowBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetCashflow : HolefeederScenario
{
    public ScenarioGetCashflow(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenNotFound()
    {
        GivenUserIsAuthorized();

        await WhenUserGetCashflow(Guid.NewGuid());

        ShouldExpectStatusCode(HttpStatusCode.NotFound);
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
            .SavedInDbAsync(DatabaseDriver);

        Category category = await GivenACategory()
            .OfType(CategoryType.Expense)
            .ForUser(HolefeederUserId)
            .SavedInDbAsync(DatabaseDriver);

        Cashflow cashflow = await GivenAnActiveCashflow()
            .ForAccount(account)
            .ForCategory(category)
            .ForUser(HolefeederUserId)
            .SavedInDbAsync(DatabaseDriver);

        GivenUserIsAuthorized();

        await WhenUserGetCashflow(cashflow.Id);

        ShouldExpectStatusCode(HttpStatusCode.OK);
        CashflowInfoViewModel? result = HttpClientDriver.DeserializeContent<CashflowInfoViewModel>();
        AssertAll(() =>
        {
            result.Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(new
                {
                    cashflow.Id,
                    cashflow.EffectiveDate,
                    cashflow.Amount,
                    cashflow.IntervalType,
                    cashflow.Frequency,
                    cashflow.Recurrence,
                    cashflow.Description,
                    cashflow.Inactive,
                    cashflow.Tags,
                    Category = new
                    {
                        category.Id,
                        category.Name,
                        category.Type,
                        category.Color
                    },
                    Account = new
                    {
                        account.Id,
                        account.Name
                    }
                });
        });
    }

    private async Task WhenUserGetCashflow(Guid id) => await HttpClientDriver.SendGetRequestAsync(ApiResources.GetCashflow, new object[] { id.ToString() });
}
