using System.Net;

using Holefeeder.Application.Features.Transactions.Queries;
using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;

using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.CashflowBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public class ScenarioGetCashflow : BaseScenario
{
    public ScenarioGetCashflow(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, testOutputHelper)
    {
        BudgetingDatabaseDriver.ResetStateAsync().Wait();
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
    public async Task WhenAuthorizedUser()
    {
        GivenUserIsAuthorized();

        await WhenUserGetCashflow(Guid.NewGuid());

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        GivenForbiddenUserIsAuthorized();

        await WhenUserGetCashflow(Guid.NewGuid());

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        GivenUserIsUnauthorized();

        await WhenUserGetCashflow(Guid.NewGuid());

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenCashflowExists()
    {
        var account = await GivenAnActiveAccount()
            .OfType(AccountType.Checking)
            .ForUser(AuthorizedUserId)
            .SavedInDb(BudgetingDatabaseDriver);

        var category = await GivenACategory()
            .OfType(CategoryType.Expense)
            .ForUser(AuthorizedUserId)
            .SavedInDb(BudgetingDatabaseDriver);

        var cashflow = await GivenAnActiveCashflow()
            .ForAccount(account)
            .ForCategory(category)
            .ForUser(AuthorizedUserId)
            .SavedInDb(BudgetingDatabaseDriver);

        GivenUserIsAuthorized();

        await WhenUserGetCashflow(cashflow.Id);

        ThenShouldExpectStatusCode(HttpStatusCode.OK);
        var result = HttpClientDriver.DeserializeContent<CashflowInfoViewModel>();
        ThenAssertAll(() =>
        {
            result.Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(cashflow, options => options.ExcludingMissingMembers());
        });
    }

    private async Task WhenUserGetCashflow(Guid id)
    {
        await HttpClientDriver.SendGetRequest(ApiResources.GetCashflow, new object?[] {id.ToString()});
    }
}
