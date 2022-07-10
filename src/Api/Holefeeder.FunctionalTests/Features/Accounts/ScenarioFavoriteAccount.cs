using System.Net;
using System.Text.Json;

using AutoBogus;

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

public class ScenarioFavoriteAccount : BaseScenario
{
    private readonly HolefeederDatabaseDriver _databaseDriver;

    public ScenarioFavoriteAccount(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
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

        await WhenUserSetsFavoriteAccount(entity);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAccountNotFound()
    {
        var entity = GivenAnActiveAccount().Build();

        GivenUserIsAuthorized();

        await WhenUserSetsFavoriteAccount(entity);

        ThenShouldExpectStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        var entity = GivenAnActiveAccount().Build();

        GivenUserIsAuthorized();

        await WhenUserSetsFavoriteAccount(entity);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        var entity = GivenAnActiveAccount().Build();

        GivenForbiddenUserIsAuthorized();

        await WhenUserSetsFavoriteAccount(entity);

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        var entity = GivenAnActiveAccount().Build();

        GivenUserIsUnauthorized();

        await WhenUserSetsFavoriteAccount(entity);

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenFavoriteAccount()
    {
        var entity = await GivenAnActiveAccount()
            .IsFavorite(false)
            .ForUser(AuthorizedUserId)
            .SavedInDb(_databaseDriver);

        entity = GivenAnExistingAccount(entity)
            .IsFavorite(true)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserSetsFavoriteAccount(entity);

        ThenShouldExpectStatusCode(HttpStatusCode.NoContent);

        var result = await _databaseDriver.FindByIdAsync<AccountEntity>(entity.Id, entity.UserId);
        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(entity);
    }

    private async Task WhenUserSetsFavoriteAccount(AccountEntity entity)
    {
        var json = JsonSerializer.Serialize(new {entity.Id, IsFavorite = entity.Favorite});
        await HttpClientDriver.SendPostRequest(ApiResources.FavoriteAccount, json);
    }
}
