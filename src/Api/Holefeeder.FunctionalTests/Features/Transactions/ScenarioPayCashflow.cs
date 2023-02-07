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

public sealed class ScenarioPayCashflow : BaseScenario
{
    public ScenarioPayCashflow(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper) : base(
        apiApplicationDriver, testOutputHelper)
    {
        if (apiApplicationDriver == null)
        {
            throw new ArgumentNullException(nameof(apiApplicationDriver));
        }

        DatabaseDriver.ResetStateAsync().Wait();
    }

    [Fact]
    public void InvalidRequest()
    {
        ScenarioFor("trying to pay a cashflow with an invalid request", player =>
        {
            Request request = default!;
            player
                .Given("", () => request = GivenAnInvalidCashflowPayment().Build())
                .Given("", () => User.IsAuthorized())
                .When("", () => Transaction.PayACashflow(request))
                .Then("", () => ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred."));
        });
    }

    [Fact]
    public void AuthorizedUser()
    {
        ScenarioFor("", player =>
        {
            Request request = null!;
            player
                .Given("", () => request = GivenACashflowPayment().Build())
                .Given("", () => User.IsAuthorized())
                .When("", () => Transaction.PayACashflow(request))
                .Then("", ThenUserShouldBeAuthorizedToAccessEndpoint);
        });
    }

    [Fact]
    public void ForbiddenUser()
    {
        ScenarioFor("", player =>
        {
            Request request = null!;
            player
                .Given("", () => request = GivenACashflowPayment().Build())
                .Given("", () => User.IsForbidden())
                .When("", () => Transaction.PayACashflow(request))
                .Then("", ShouldBeForbiddenToAccessEndpoint);
        });
    }

    [Fact]
    public void UnauthorizedUser()
    {
        ScenarioFor("", player =>
        {
            Request entity = null!;
            player
                .Given("", () => entity = GivenACashflowPayment().Build())
                .Given("", () => User.IsUnauthorized())
                .When("", () => Transaction.PayACashflow(entity))
                .Then("", ShouldNotBeAuthorizedToAccessEndpoint);
        });
    }

    [Fact]
    public void ValidRequest()
    {
        ScenarioFor("", player =>
        {
            Account account = null!;
            Category category = null!;
            Cashflow cashflow = null!;
            Request request = null!;
            var id = Guid.Empty;

            player
                .Given("", async () => account = await GivenAnActiveAccount()
                    .ForUser(AuthorizedUserId)
                    .SavedInDb(DatabaseDriver))
                .Given("", async () => category = await GivenACategory()
                    .ForUser(AuthorizedUserId)
                    .SavedInDb(DatabaseDriver))
                .Given("", async () => cashflow = await GivenAnActiveCashflow()
                    .ForAccount(account)
                    .ForCategory(category)
                    .ForUser(AuthorizedUserId)
                    .SavedInDb(DatabaseDriver))
                .Given("", () => request = GivenACashflowPayment()
                    .ForCashflow(cashflow)
                    .Build())
                .Given("", () => User.IsAuthorized())
                .When("", () => Transaction.PayACashflow(request))
                .Then("", () => ThenShouldExpectStatusCode(HttpStatusCode.Created))
                .Then("", () => id = ThenShouldGetTheRouteOfTheNewResourceInTheHeader())
                .Then("", async () =>
                {
                    var result = await DatabaseDriver.FindByIdAsync<Transaction>(id);

                    TransactionMapper.MapToModelOrNull(result).Should()
                        .NotBeNull()
                        .And
                        .BeEquivalentTo(request, options => options.ExcludingMissingMembers());
                });
        });
    }
}