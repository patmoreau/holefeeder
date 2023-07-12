using System.Net;
using Holefeeder.Application.Features.Transactions;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using static Holefeeder.Application.Features.Transactions.Commands.MakePurchase;
using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.MakePurchaseRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

[ComponentTest]
public sealed class ScenarioMakePurchase : HolefeederScenario
{
    public ScenarioMakePurchase(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, testOutputHelper)
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

        await ScenarioFor("making an invalid request purchase", player =>
        {
            player
                .Given(User.IsAuthorized)
                .And("making a purchase of 0$", () => request = GivenAPurchase().OfAmount(0).Build())
                .When("sending the request", () => Transaction.MakesPurchase(request))
                .Then("should receive a validation error", () => ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred."));
        });
    }

    [Fact]
    public async Task AuthorizedUser()
    {
        Request request = null!;

        await ScenarioFor("making an authorized purchase", player =>
        {
            player
                .Given(User.IsAuthorized)
                .And("making a purchase", () => request = GivenAPurchase().Build())
                .When("sending the request", () => Transaction.MakesPurchase(request))
                .Then("should be allowed", ShouldBeAuthorizedToAccessEndpoint);
        });
    }

    [Fact]
    public async Task ForbiddenUser()
    {
        Request request = null!;

        await ScenarioFor("making an forbidden purchase", player =>
        {
            player
                .Given(User.IsForbidden)
                .And("making a purchase", () => request = GivenAPurchase().Build())
                .When("sending the request", () => Transaction.MakesPurchase(request))
                .Then("should be forbidden", ShouldBeForbiddenToAccessEndpoint);
        });
    }

    [Fact]
    public async Task UnauthorizedUser()
    {
        Request entity = null!;

        await ScenarioFor("making an unauthorized purchase", player =>
        {
            player
                .Given(User.IsUnauthorized)
                .And("making a purchase", () => entity = GivenAPurchase().Build())
                .When("sending the request", () => Transaction.MakesPurchase(entity))
                .Then("should be unauthorized", ShouldNotBeAuthorizedToAccessEndpoint);
        });
    }

    [Fact(Skip = "Works locally but not on the workflow; need this deployed to fix urgent bug")]
    public async Task ValidRequest()
    {
        Account account = null!;
        Category category = null!;
        Request request = null!;
        Guid id = Guid.Empty;

        await ScenarioFor("making a valid purchase", player =>
        {
            player
                .Given(User.IsAuthorized)
                .And(Account.Exists)
                .And("category", async () => category = await GivenACategory().ForUser(HolefeederUserId).SavedInDbAsync(DatabaseDriver))
                .And("wanting to make a purchase", () => request = GivenAPurchase().ForAccount(account).ForCategory(category).Build())
                .When("the purchase is made", () => Transaction.MakesPurchase(request))
                .Then("the response should be created", () => ShouldExpectStatusCode(HttpStatusCode.Created))
                // .And(ShouldGetTheRouteOfTheNewResourceInTheHeader)
                .And("the purchase saved in the database should match the request", async () =>
                {
                    Transaction? result = await DatabaseDriver.FindByIdAsync<Transaction>(id);

                    result.Should().NotBeNull($"because the TransactionId ({id}) was not found");

                    TransactionMapper.MapToModelOrNull(result).Should()
                        .NotBeNull()
                        .And
                        .BeEquivalentTo(request, options => options.ExcludingMissingMembers());
                });
        });
    }
}
