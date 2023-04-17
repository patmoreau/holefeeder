using System.Net;
using System.Text.Json;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.Application.Features.Accounts.Commands.CloseAccount;
using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Accounts.CloseAccountRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Accounts;

public class ScenarioCloseAccount : BaseScenario
{
    public ScenarioCloseAccount(ApiApplicationDriver applicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        Request request = default!;

        await ScenarioFor("closing an account with an invalid request", player =>
        {
            player
                .Given("an invalid account request", () => request = GivenAnInvalidCloseAccountRequest().Build())
                .Given("the user is authorized", () => GivenUserIsAuthorized())
                .When("the user closes an account", () => WhenUserClosesAccount(request))
                .Then("should receive a validation error", () => ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred."));
        });
    }

    [Fact]
    public async Task WhenAccountNotFound()
    {
        Request request = default!;

        await ScenarioFor("closing an account that does not exists", player =>
        {
            player
                .Given("a close account request", () => request = GivenACloseAccountRequest().Build())
                .Given("the user is authorized", () => GivenUserIsAuthorized())
                .When("the user closes an account", () => WhenUserClosesAccount(request))
                .Then("should receive a NotFound error code", () => ThenShouldExpectStatusCode(HttpStatusCode.NotFound));
        });
    }

    [Fact]
    public async Task WhenCloseAccount()
    {
        Account entity = await GivenAnActiveAccount()
            .ForUser(HolefeederUserId)
            .SavedInDb(DatabaseDriver);

        Request request = GivenACloseAccountRequest()
            .WithId(entity.Id)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserClosesAccount(request);

        ThenShouldExpectStatusCode(HttpStatusCode.NoContent);

        Account? result = await DatabaseDriver.FindByIdAsync<Account>(entity.Id);
        result.Should().NotBeNull();
        result!.Inactive.Should().BeTrue();
    }

    private async Task WhenUserClosesAccount(Request request)
    {
        string json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResources.CloseAccount, json);
    }
}
