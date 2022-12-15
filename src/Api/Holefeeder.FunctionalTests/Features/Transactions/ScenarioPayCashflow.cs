using System.Net;

using Holefeeder.Application.Features.Transactions;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;

using static Holefeeder.Application.Features.Transactions.Commands.PayCashflow;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.CashflowBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.PayCashflowRequestBuilder;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public sealed class ScenarioPayCashflow : BaseScenario<ScenarioPayCashflow>
{

    public ScenarioPayCashflow(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper) : base(
        apiApplicationDriver, testOutputHelper)
    {
        if (apiApplicationDriver == null)
        {
            throw new ArgumentNullException(nameof(apiApplicationDriver));
        }

        BudgetingDatabaseDriver.ResetStateAsync().Wait();
    }

    [Fact]
    public async Task InvalidRequest()
    {
        Request request = default!;
        await Given(() =>
            {
                request = GivenAnInvalidCashflowPayment().Build();
            })
            .Given(() => User.IsAuthorized())
            .When(() => Transaction.PayACashflow(request))
            .Then(
                () => ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred."))
            .RunScenarioAsync();
    }

    [Fact]
    public async Task AuthorizedUser()
    {
        Request request = null!;

        await Given(() => request = GivenACashflowPayment().Build())
            .Given(() => User.IsAuthorized())
            .When(() => Transaction.PayACashflow(request))
            .Then(ThenUserShouldBeAuthorizedToAccessEndpoint)
            .RunScenarioAsync();
    }

    [Fact]
    public async Task ForbiddenUser()
    {
        Request request = null!;
        await Given(() => request = GivenACashflowPayment().Build())
            .Given(() => User.IsForbidden())
            .When(() => Transaction.PayACashflow(request))
            .Then(ShouldBeForbiddenToAccessEndpoint)
            .RunScenarioAsync();
    }

    [Fact]
    public async Task UnauthorizedUser()
    {
        Request entity = null!;
        await Given(() => entity = GivenACashflowPayment().Build())
            .Given(() => User.IsUnauthorized())
            .When(() => Transaction.PayACashflow(entity))
            .Then(ShouldNotBeAuthorizedToAccessEndpoint)
            .RunScenarioAsync();
    }

    [Fact]
    public async Task ValidRequest()
    {
        Account account = null!;
        Category category = null!;
        Cashflow cashflow = null!;
        Request request = null!;
        var id = Guid.Empty;

        await Given(async () => account = await GivenAnActiveAccount()
                .ForUser(AuthorizedUserId)
                .SavedInDb(BudgetingDatabaseDriver))
            .Given(async () => category = await GivenACategory()
                .ForUser(AuthorizedUserId)
                .SavedInDb(BudgetingDatabaseDriver))
            .Given(async () => cashflow = await GivenAnActiveCashflow()
                .ForAccount(account)
                .ForCategory(category)
                .ForUser(AuthorizedUserId)
                .SavedInDb(BudgetingDatabaseDriver))
            .Given(() => request = GivenACashflowPayment()
                .ForCashflow(cashflow)
                .Build())
            .Given(() => User.IsAuthorized())
            .When(() => Transaction.PayACashflow(request))
            .Then(() => ThenShouldExpectStatusCode(HttpStatusCode.Created))
            .Then(() => id = ThenShouldGetTheRouteOfTheNewResourceInTheHeader())
            .Then(async () =>
            {
                var result = await BudgetingDatabaseDriver.FindByIdAsync<Transaction>(id);

                TransactionMapper.MapToModelOrNull(result).Should()
                    .NotBeNull()
                    .And
                    .BeEquivalentTo(request, options => options.ExcludingMissingMembers());
            })
            .RunScenarioAsync();
    }
}
