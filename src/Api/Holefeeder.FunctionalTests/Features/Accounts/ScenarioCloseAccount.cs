using System.Net;
using System.Text.Json;

using FluentAssertions;

using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.Infrastructure.Entities;

using Xunit;
using Xunit.Abstractions;

using static Holefeeder.Tests.Common.Builders.AccountEntityBuilder;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.Accounts;

public class ScenarioCloseAccount : BaseScenario
{
    private readonly HolefeederDatabaseDriver _databaseDriver;

    public ScenarioCloseAccount(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, testOutputHelper)
    {
        _databaseDriver = apiApplicationDriver.CreateHolefeederDatabaseDriver();
        _databaseDriver.ResetStateAsync().Wait();
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        var entity = GivenAnActiveAccount()
            .WithId(Guid.Empty)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserClosesAccount(entity);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAccountNotFound()
    {
        var entity = GivenAnActiveAccount().Build();

        GivenUserIsAuthorized();

        await WhenUserClosesAccount(entity);

        ThenShouldExpectStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        var entity = GivenAnActiveAccount().Build();

        GivenUserIsAuthorized();

        await WhenUserClosesAccount(entity);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        var entity = GivenAnActiveAccount().Build();

        GivenForbiddenUserIsAuthorized();

        await WhenUserClosesAccount(entity);

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        var entity = GivenAnActiveAccount().Build();

        GivenUserIsUnauthorized();

        await WhenUserClosesAccount(entity);

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenCloseAccount()
    {
        var entity = await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .SavedInDb(_databaseDriver);

        GivenUserIsAuthorized();

        await WhenUserClosesAccount(entity);

        ThenShouldExpectStatusCode(HttpStatusCode.NoContent);

        var result = await _databaseDriver.FindByIdAsync<AccountEntity>(entity.Id, entity.UserId);
        result.Should().NotBeNull();
        result!.Inactive.Should().BeTrue();
    }

    private async Task WhenUserClosesAccount(AccountEntity entity)
    {
        var json = JsonSerializer.Serialize(new {entity.Id});
        await HttpClientDriver.SendPostRequest(ApiResources.CloseAccount, json);
    }
}
