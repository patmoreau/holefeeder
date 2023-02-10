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
        Request request = default!;

        ScenarioFor("making an invalid request purchase", player =>
        {
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
        Request request = null!;

        ScenarioFor("making an authorized purchase", player =>
        {
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
        Request request = null!;

        ScenarioFor("making an forbidden purchase", player =>
        {
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
        Request entity = null!;

        ScenarioFor("making an unauthorized purchase", player =>
        {
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
        Account account = null!;
        Category category = null!;
        Request request = null!;
        var id = Guid.Empty;

        ScenarioFor("making a valid purchase", player =>
        {
            player
                .Given("the user is authorized", () => User.IsAuthorized())
                .And("has an active account", async () => account = await GivenAnActiveAccount().ForUser(AuthorizedUserId).SavedInDb(DatabaseDriver))
                .And("category", async () => category = await GivenACategory().ForUser(AuthorizedUserId).SavedInDb(DatabaseDriver))
                .And("wanting to make a purchase", () => request = GivenAPurchase().ForAccount(account).ForCategory(category).Build())
                .When("the purchase is made", () => Transaction.MakesPurchase(request))
                .Then("the response should be created", () => ThenShouldExpectStatusCode(HttpStatusCode.Created))
                .And("have the resource link in the header", () =>
                {
                    id = ThenShouldGetTheRouteOfTheNewResourceInTheHeader();

                    id.Should().NotBeEmpty();
                })
                .And("the purchase saved in the database should match the request", async () =>
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
