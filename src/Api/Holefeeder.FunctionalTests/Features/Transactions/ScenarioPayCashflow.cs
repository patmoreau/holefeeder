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
using static Holefeeder.Tests.Common.SeedWork.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public sealed class ScenarioPayCashflow : BaseScenario
{
    public ScenarioPayCashflow(ApiApplicationDriver applicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
        if (applicationDriver == null)
        {
            throw new ArgumentNullException(nameof(applicationDriver));
        }
    }

    [Fact]
    public async Task InvalidRequest()
    {
        Request request = default!;

        await ScenarioFor("trying to pay a cashflow with an invalid request", player =>
        {
            player
                .Given(User.IsAuthorized)
                .And("creates an invalid payment", () => request = GivenAnInvalidCashflowPayment().Build())
                .When("sending the request", () => Transaction.PayACashflow(request))
                .Then("should receive a validation error", () => ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred."));
        });
    }

    [Fact]
    public async Task AuthorizedUser()
    {
        Request request = null!;

        await ScenarioFor("an authorized user pays a cashflow", player =>
        {
            player
                .Given(User.IsAuthorized)
                .And("a cashflow payment request", () => request = GivenACashflowPayment().Build())
                .When("the payment is sent", () => Transaction.PayACashflow(request))
                .Then("the user should be authorized", ThenUserShouldBeAuthorizedToAccessEndpoint);
        });
    }

    [Fact]
    public async Task ForbiddenUser()
    {
        Request request = null!;

        await ScenarioFor("a forbidden user pays a cashflow", player =>
        {
            player
                .Given(User.IsForbidden)
                .And("a cashflow payment request", () => request = GivenACashflowPayment().Build())
                .When("the payment is sent", () => Transaction.PayACashflow(request))
                .Then("the user should be forbidden", ShouldBeForbiddenToAccessEndpoint);
        });
    }

    [Fact]
    public async Task UnauthorizedUser()
    {
        Request entity = null!;

        await ScenarioFor("an unauthorized user pays a cashflow", player =>
        {
            player
                .Given(User.IsUnauthorized)
                .And("a cashflow payment request", () => entity = GivenACashflowPayment().Build())
                .When("the payment is sent", () => Transaction.PayACashflow(entity))
                .Then("the user should not be authorized", ShouldNotBeAuthorizedToAccessEndpoint);
        });
    }

    [Fact]
    public async Task ValidRequest()
    {
        Account account = null!;
        Category category = null!;
        Cashflow cashflow = null!;
        Request request = null!;
        Guid id = Guid.Empty;

        await ScenarioFor("paying a cashflow", player =>
        {
            player
                .Given(User.IsAuthorized)
                .And("has an active account", async () => account = await GivenAnActiveAccount().ForUser(AuthorizedUserId).SavedInDb(DatabaseDriver))
                .And("a category", async () => category = await GivenACategory().ForUser(AuthorizedUserId).SavedInDb(DatabaseDriver))
                .And("a cashflow setup", async () => cashflow = await GivenAnActiveCashflow().ForAccount(account).ForCategory(category).ForUser(AuthorizedUserId).SavedInDb(DatabaseDriver))
                .And("and wanting to pay a cashflow", () => request = GivenACashflowPayment().ForCashflow(cashflow).ForDate(cashflow.EffectiveDate).Build())
                .When("the payment is made", () => Transaction.PayACashflow(request))
                .Then("the response should be created", () => ThenShouldExpectStatusCode(HttpStatusCode.Created))
                .And("have the resource link in the header", () => id = ThenShouldGetTheRouteOfTheNewResourceInTheHeader())
                .And("the cashflow paid saved in the database should match the request", async () =>
                {
                    Transaction? result = await DatabaseDriver.FindByIdAsync<Transaction>(id);

                    result.Should().NotBeNull();

                    TransactionMapper.MapToModelOrNull(result).Should()
                        .NotBeNull()
                        .And
                        .BeEquivalentTo(request, options => options.ExcludingMissingMembers());
                });
        });
    }
}
