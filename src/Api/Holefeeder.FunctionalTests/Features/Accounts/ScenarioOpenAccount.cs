using System.Net;
using System.Text.Json;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;

using static Holefeeder.Application.Features.Accounts.Commands.OpenAccount;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Accounts.OpenAccountRequestBuilder;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.Accounts;

public class ScenarioOpenAccount : BaseScenario
{
    public ScenarioOpenAccount(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, testOutputHelper)
    {
        BudgetingDatabaseDriver.ResetStateAsync().Wait();
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        var request = GivenAnInvalidOpenAccountRequest()
            .Build();

        GivenUserIsAuthorized();

        await WhenUserOpensAnAccount(request);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAccountNameAlreadyExistsRequest()
    {
        var entity = await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .SavedInDb(BudgetingDatabaseDriver);

        var request = GivenAnOpenAccountRequest()
            .WithName(entity.Name)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserOpensAnAccount(request);

        ThenShouldExpectStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        var request = GivenAnOpenAccountRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserOpensAnAccount(request);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        var request = GivenAnOpenAccountRequest().Build();

        GivenForbiddenUserIsAuthorized();

        await WhenUserOpensAnAccount(request);

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        var request = GivenAnOpenAccountRequest().Build();

        GivenUserIsUnauthorized();

        await WhenUserOpensAnAccount(request);

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenOpenAccount()
    {
        var request = GivenAnOpenAccountRequest()
            .Build();

        GivenUserIsAuthorized();

        await WhenUserOpensAnAccount(request);

        ThenShouldExpectStatusCode(HttpStatusCode.Created);

        var id = ThenShouldGetTheRouteOfTheNewResourceInTheHeader();

        var result = await BudgetingDatabaseDriver.FindByIdAsync<Account>(id);
        result.Should().NotBeNull().And.BeEquivalentTo(request, options => options.ExcludingMissingMembers());
    }

    private async Task WhenUserOpensAnAccount(Request request)
    {
        var json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResources.OpenAccount, json);
    }
}
