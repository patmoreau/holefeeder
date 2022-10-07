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

public class ScenarioModifyAccount : BaseScenario
{
    private readonly HolefeederDatabaseDriver _databaseDriver;

    public ScenarioModifyAccount(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
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

        await WhenUserModifiesAccount(entity);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAccountNotFound()
    {
        var entity = GivenAnActiveAccount().Build();

        GivenUserIsAuthorized();

        await WhenUserModifiesAccount(entity);

        ThenShouldExpectStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        var entity = GivenAnActiveAccount().Build();

        GivenUserIsAuthorized();

        await WhenUserModifiesAccount(entity);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        var entity = GivenAnActiveAccount().Build();

        GivenForbiddenUserIsAuthorized();

        await WhenUserModifiesAccount(entity);

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        var entity = GivenAnActiveAccount().Build();

        GivenUserIsUnauthorized();

        await WhenUserModifiesAccount(entity);

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenModifyAccount()
    {
        var entity = await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .SavedInDb(_databaseDriver);

        entity = GivenAnExistingAccount(entity)
            .WithName("modified name")
            .WithDescription("modified description")
            .WithOpenBalance(123.45m)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserModifiesAccount(entity);

        ThenShouldExpectStatusCode(HttpStatusCode.NoContent);

        var result = await _databaseDriver.FindByIdAsync<AccountEntity>(entity.Id, entity.UserId);
        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(entity);
    }

    private async Task WhenUserModifiesAccount(AccountEntity entity)
    {
        var json = JsonSerializer.Serialize(new {entity.Id, entity.Name, entity.OpenBalance, entity.Description});
        await HttpClientDriver.SendPostRequest(ApiResources.ModifyAccount, json);
    }
}
