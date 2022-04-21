using System.Net;

using FluentAssertions;

using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

using Microsoft.AspNetCore.Mvc;

using Xunit;

namespace Holefeeder.FunctionalTests.Features.Accounts;

public class GetAccountTests : BaseFeatureTests, IClassFixture<HttpClientDriver>
{
    public GetAccountTests(HttpClientDriver httpClientDriver) : base(httpClientDriver)
    {
    }

    [Fact]
    public async Task Scenario_NotFound()
    {
        GivenUserIsAuthorized();

        await WhenUserGetAccount(Guid.NewGuid());

        ThenShouldExpectStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Scenario_InvalidRequest()
    {
        GivenUserIsAuthorized();

        await WhenUserGetAccount(Guid.Empty);

        await ThenShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task Scenario_AuthorizedUser()
    {
        GivenUserIsAuthorized();

        await WhenUserGetAccount(Guid.NewGuid());

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task Scenario_ForbiddenUser()
    {
        GivenForbiddenUserIsAuthorized();

        await WhenUserGetAccount(Guid.NewGuid());

        ThenUserShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task Scenario_UnauthorizedUser()
    {
        GivenUserIsUnauthorized();

        await WhenUserGetAccount(Guid.NewGuid());

        ThenUserShouldNotBeAuthorizedToAccessEndpoint();
    }

    private async Task WhenUserGetAccount(Guid id)
    {
        await HttpClientDriver.SendGetRequest(ApiResources.GetAccount, new[] {id.ToString()});
    }
}
