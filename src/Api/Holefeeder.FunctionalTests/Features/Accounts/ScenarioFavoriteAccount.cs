using System.Net;
using System.Text.Json;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using static Holefeeder.Application.Features.Accounts.Commands.FavoriteAccount;
using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Accounts.FavoriteAccountRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Accounts;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioFavoriteAccount : HolefeederScenario
{
    public ScenarioFavoriteAccount(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        Request request = GivenAnInvalidFavoriteAccountRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserSetsFavoriteAccount(request);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAccountNotFound()
    {
        Request request = GivenAFavoriteAccountRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserSetsFavoriteAccount(request);

        ShouldExpectStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenFavoriteAccount()
    {
        Account entity = await GivenAnActiveAccount()
            .ForUser(HolefeederUserId)
            .IsFavorite(false)
            .SavedInDbAsync(DatabaseDriver);

        Request request = GivenAFavoriteAccountRequest()
            .WithId(entity.Id)
            .IsFavorite()
            .Build();

        GivenUserIsAuthorized();

        await WhenUserSetsFavoriteAccount(request);

        ShouldExpectStatusCode(HttpStatusCode.NoContent);

        using var dbContext = DatabaseDriver.CreateDbContext();

        Account? result = await dbContext.FindByIdAsync<Account>(entity.Id);
        result.Should().NotBeNull();
        result!.Favorite.Should().BeTrue();
    }

    private async Task WhenUserSetsFavoriteAccount(Request request)
    {
        string json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequestAsync(ApiResources.FavoriteAccount, json);
    }
}
