using System.Net;

using FluentAssertions;

using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;

using Xunit;

using static Holefeeder.Tests.Common.Builders.AccountEntityBuilder;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.Accounts;

public class ScenarioGetAccounts : BaseScenario
{
    private readonly HolefeederDatabaseDriver _holefeederDatabaseDriver;

    public ScenarioGetAccounts(ApiApplicationDriver apiApplicationDriver) : base(apiApplicationDriver)
    {
        _holefeederDatabaseDriver = apiApplicationDriver.CreateHolefeederDatabaseDriver();
        _holefeederDatabaseDriver.ResetStateAsync().Wait();
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        GivenUserIsAuthorized();

        await WhenUserTriesToQuery(ApiResources.GetAccounts, offset: -1);

        ThenShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        GivenUserIsAuthorized();

        await WhenUserTriesToQuery(ApiResources.GetAccounts);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        GivenForbiddenUserIsAuthorized();

        await WhenUserTriesToQuery(ApiResources.GetAccounts);

        ThenUserShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        GivenUserIsUnauthorized();

        await WhenUserTriesToQuery(ApiResources.GetAccounts);

        ThenUserShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenAccountsExistsSortedByNameDesc()
    {
        const string firstName = nameof(firstName);
        const string secondName = nameof(secondName);

        await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .WithName(firstName)
            .SavedInDb(_holefeederDatabaseDriver);

        await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .WithName(secondName)
            .SavedInDb(_holefeederDatabaseDriver);

        await GivenAnActiveAccount()
            .SavedInDb(_holefeederDatabaseDriver);

        GivenUserIsAuthorized();

        await WhenUserTriesToQuery(ApiResources.GetAccounts, sorts: "-name");

        ThenShouldExpectStatusCode(HttpStatusCode.OK);
        var result = HttpClientDriver.DeserializeContent<AccountViewModel[]>();
        ThenAssertAll(() =>
        {
            result.Should().NotBeNull().And.HaveCount(2);
            result![0].Name.Should().Be(secondName);
            result[1].Name.Should().Be(firstName);
        });
    }
}
