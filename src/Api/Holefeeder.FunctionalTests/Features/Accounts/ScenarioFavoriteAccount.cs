using System.Net;
using System.Text.Json;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;

using static Holefeeder.Application.Features.Accounts.Commands.FavoriteAccount;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Accounts.FavoriteAccountRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Accounts;

public class ScenarioFavoriteAccount : BaseScenario
{
    public ScenarioFavoriteAccount(ApiApplicationDriver apiApplicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        var request = GivenAnInvalidFavoriteAccountRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserSetsFavoriteAccount(request);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAccountNotFound()
    {
        var request = GivenAFavoriteAccountRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserSetsFavoriteAccount(request);

        ThenShouldExpectStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        var request = GivenAFavoriteAccountRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserSetsFavoriteAccount(request);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        var entity = GivenAFavoriteAccountRequest().Build();

        GivenForbiddenUserIsAuthorized();

        await WhenUserSetsFavoriteAccount(entity);

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        var entity = GivenAFavoriteAccountRequest().Build();

        GivenUserIsUnauthorized();

        await WhenUserSetsFavoriteAccount(entity);

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenFavoriteAccount()
    {
        var entity = await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .IsFavorite(false)
            .SavedInDb(DatabaseDriver);

        var request = GivenAFavoriteAccountRequest()
            .WithId(entity.Id)
            .IsFavorite()
            .Build();

        GivenUserIsAuthorized();

        await WhenUserSetsFavoriteAccount(request);

        ThenShouldExpectStatusCode(HttpStatusCode.NoContent);

        var result = await DatabaseDriver.FindByIdAsync<Account>(entity.Id);
        result.Should().NotBeNull();
        result!.Favorite.Should().BeTrue();
    }

    private async Task WhenUserSetsFavoriteAccount(Request request)
    {
        var json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResources.FavoriteAccount, json);
    }
}
