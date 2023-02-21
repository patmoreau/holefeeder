using System.Net;
using System.Text.Json;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;

using static Holefeeder.Application.Features.Accounts.Commands.CloseAccount;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Accounts.CloseAccountRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Accounts;

public class ScenarioCloseAccount : BaseScenario
{
    public ScenarioCloseAccount(ApiApplicationDriver apiApplicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, budgetingDatabaseInitializer, testOutputHelper)
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
    public async Task WhenAuthorizedUser()
    {
        var request = GivenACloseAccountRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserClosesAccount(request);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        var request = GivenACloseAccountRequest().Build();

        GivenForbiddenUserIsAuthorized();

        await WhenUserClosesAccount(request);

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        var request = GivenACloseAccountRequest().Build();

        GivenUserIsUnauthorized();

        await WhenUserClosesAccount(request);

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenCloseAccount()
    {
        var entity = await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .SavedInDb(DatabaseDriver);

        var request = GivenACloseAccountRequest()
            .WithId(entity.Id)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserClosesAccount(request);

        ThenShouldExpectStatusCode(HttpStatusCode.NoContent);

        var result = await DatabaseDriver.FindByIdAsync<Account>(entity.Id);
        result.Should().NotBeNull();
        result!.Inactive.Should().BeTrue();
    }

    private async Task WhenUserClosesAccount(Request request)
    {
        var json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResources.CloseAccount, json);
    }
}
