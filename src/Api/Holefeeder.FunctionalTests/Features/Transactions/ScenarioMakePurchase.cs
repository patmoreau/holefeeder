using System.Net;

using Holefeeder.Application.Features.Transactions;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;

using static Holefeeder.Application.Features.Transactions.Commands.MakePurchase;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.MakePurchaseRequestBuilder;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public sealed class ScenarioMakePurchase : BaseScenario
{
    public ScenarioMakePurchase(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper) : base(
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
        ScenarioFor("making an invalid request purchase", player =>
        {
            Request request = default!;
            player
                .Given("an authorized user", () => User.IsAuthorized())
                .And("making a purchase of 0$", () => request = GivenAPurchase().OfAmount(0).Build())
                .When("sending the request", () => Transaction.MakesPurchase(request))
                .Then("should receive a validation error", () => ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred."));
        });
    }

    [Fact]
    public void AuthorizedUser()
    {
        ScenarioFor("making an authorized purchase", player =>
        {
            Request request = null!;

            player
                .Given("an authorized user", () => User.IsAuthorized())
                .And("making a purchase", () => request = GivenAPurchase().Build())
                .When("sending the request", () => Transaction.MakesPurchase(request))
                .Then("should be allowed", ThenUserShouldBeAuthorizedToAccessEndpoint);
        });
    }

    [Fact]
    public void ForbiddenUser()
    {
        ScenarioFor("making an forbidden purchase", player =>
        {
            Request request = null!;
            player
                .Given("a forbidden user", () => User.IsForbidden())
                .And("making a purchase", () => request = GivenAPurchase().Build())
                .When("sending the request", () => Transaction.MakesPurchase(request))
                .Then("should be forbidden", ShouldBeForbiddenToAccessEndpoint);
        });
    }

    [Fact]
    public void UnauthorizedUser()
    {
        ScenarioFor("making an unauthorized purchase", player =>
        {
            Request entity = null!;
            player
                .Given("an unauthorized user", () => User.IsUnauthorized())
                .And("making a purchase", () => entity = GivenAPurchase().Build())
                .When("sending the request", () => Transaction.MakesPurchase(entity))
                .Then("should be unauthorized", ShouldNotBeAuthorizedToAccessEndpoint);
        });
    }

    [Fact]
    public void ValidRequest()
    {
        ScenarioFor("making a valid purchase", player =>
        {
            Account account = null!;
            Category category = null!;
            Request request = null!;
            var id = Guid.Empty;

            player
                .Given("", async () => account = await GivenAnActiveAccount()
                    .ForUser(AuthorizedUserId)
                    .SavedInDb(DatabaseDriver))
                .Given("", async () => category = await GivenACategory()
                    .ForUser(AuthorizedUserId)
                    .SavedInDb(DatabaseDriver))
                .Given("", () => request = GivenAPurchase()
                    .ForAccount(account)
                    .ForCategory(category)
                    .Build())
                .Given("", () => User.IsAuthorized())
                .When("", () => Transaction.MakesPurchase(request))
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
