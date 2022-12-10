using System.Net;
using System.Text.Json;

using FluentAssertions;

using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.Infrastructure.Entities;

using static Holefeeder.Tests.Common.Builders.AccountEntityBuilder;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.Accounts;

public class ScenarioOpenAccount : BaseScenario
{
    private readonly HolefeederDatabaseDriver _databaseDriver;

    public ScenarioOpenAccount(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, testOutputHelper)
    {
        _databaseDriver = HolefeederDatabaseDriver;
        _databaseDriver.ResetStateAsync().Wait();
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        var entity = GivenAnActiveAccount()
            .WithName(string.Empty)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserOpensAnAccount(entity);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        var entity = GivenAnActiveAccount().Build();

        GivenUserIsAuthorized();

        await WhenUserOpensAnAccount(entity);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        var entity = GivenAnActiveAccount().Build();

        GivenForbiddenUserIsAuthorized();

        await WhenUserOpensAnAccount(entity);

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        var entity = GivenAnActiveAccount().Build();

        GivenUserIsUnauthorized();

        await WhenUserOpensAnAccount(entity);

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenOpenAccount()
    {
        var entity = GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserOpensAnAccount(entity);

        ThenShouldExpectStatusCode(HttpStatusCode.Created);

        var id = ThenShouldGetTheRouteOfTheNewResourceInTheHeader();

        var result = await _databaseDriver.FindByIdAsync<AccountEntity>(id, entity.UserId);
        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(entity,
            options => options.Excluding(info => info.Id).Excluding(info => info.Favorite));
    }

    private async Task WhenUserOpensAnAccount(AccountEntity entity)
    {
        var json = JsonSerializer.Serialize(new
        {
            entity.Type,
            entity.Name,
            entity.OpenBalance,
            entity.OpenDate,
            entity.Description
        });
        await HttpClientDriver.SendPostRequest(ApiResources.OpenAccount, json);
    }
}
