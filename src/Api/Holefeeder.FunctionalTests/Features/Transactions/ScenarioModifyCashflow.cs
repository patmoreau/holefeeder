using System.Net;
using System.Text.Json;

using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;

using static Holefeeder.Application.Features.Transactions.Commands.ModifyCashflow;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.CashflowBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.ModifyCashflowRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public class ScenarioModifyCashflow : BaseScenario
{
    public ScenarioModifyCashflow(ApiApplicationDriver apiApplicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
        if (apiApplicationDriver == null)
        {
            throw new ArgumentNullException(nameof(apiApplicationDriver));
        }
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        var request = GivenAnInvalidModifyCashflowRequest()
            .OfAmount(Decimal.MinusOne)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserModifiedACashflow(request);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        var request = GivenAModifyCashflowRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserModifiedACashflow(request);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        var request = GivenAModifyCashflowRequest().Build();

        GivenForbiddenUserIsAuthorized();

        await WhenUserModifiedACashflow(request);

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        var request = GivenAModifyCashflowRequest().Build();

        GivenUserIsUnauthorized();

        await WhenUserModifiedACashflow(request);

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenModifyACashflow()
    {
        var account = await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .SavedInDb(DatabaseDriver);

        var category = await GivenACategory()
            .ForUser(AuthorizedUserId)
            .SavedInDb(DatabaseDriver);

        var cashflow = await GivenAnActiveCashflow()
            .ForAccount(account)
            .ForCategory(category)
            .ForUser(AuthorizedUserId)
            .SavedInDb(DatabaseDriver);

        GivenUserIsAuthorized();

        var request = GivenAModifyCashflowRequest()
            .WithId(cashflow.Id)
            .Build();

        await WhenUserModifiedACashflow(request);

        ThenShouldExpectStatusCode(HttpStatusCode.NoContent);

        var result = await DatabaseDriver.FindByIdAsync<Cashflow>(cashflow.Id);

        result.Should().NotBeNull().And.BeEquivalentTo(request, options => options.ExcludingMissingMembers());
    }

    private async Task WhenUserModifiedACashflow(Request request)
    {
        var json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResources.ModifyCashflow, json);
    }
}
