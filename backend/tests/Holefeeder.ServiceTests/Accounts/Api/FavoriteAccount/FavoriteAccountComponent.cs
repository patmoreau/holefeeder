using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;
using Holefeeder.ServiceTests.TestKit.Persistence;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Holefeeder.ServiceTests.Accounts.Api.FavoriteAccount;

[Collection(ComponentTestMarker.Name)]
public class FavoriteAccountComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private const string ValidationErrorTitle = "One or more validation errors occurred.";

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _amex;

    [Fact]
    public Task WhenTheIdIsEmpty() =>
        ScenarioRunner.Create("Setting the favorite of an account with an empty id", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user sets the favorite of an account with an empty id", () => SettingTheFavorite(Guid.Empty, true))
            .Then<HttpResponseMessage>("a validation error is returned on the id", AValidationErrorIsReturnedOnTheId)
            .PlayAsync();

    [Fact]
    public Task WhenTheAccountBelongsToAnotherUser() =>
        ScenarioRunner.Create("Setting the favorite of another user's account", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("another user has a credit card", AnotherUserHasACreditCard)
            .When("the user sets the other user's credit card as favorite", () => SettingTheFavorite(_amex, true))
            .Then<HttpResponseMessage>("the account is not found", TheAccountIsNotFound)
            .PlayAsync();

    [Fact]
    public Task WhenTheAccountBecomesAFavorite() =>
        ScenarioRunner.Create("Setting an account as favorite", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a credit card that is not a favorite", TheUserHasACreditCard)
            .When("the user sets the credit card as favorite", () => SettingTheFavorite(_amex, true))
            .Then<HttpResponseMessage>("the credit card is a favorite", TheCreditCardIsAFavorite)
            .PlayAsync();

    private async Task ARegisteredUser() =>
        _user = await AUser().SavedInDbAsync(server.Database);

    private async Task TheUserHasACreditCard() =>
        _amex = await new BudgetForTest(_user, server.Database).AnAccountAsync("Amex", AccountType.CreditCard, favorite: false);

    private async Task AnotherUserHasACreditCard()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        _amex = await new BudgetForTest(otherUser, server.Database).AnAccountAsync("Amex", AccountType.CreditCard);
    }

    private async Task<HttpResponseMessage> SettingTheFavorite(Guid id, bool isFavorite)
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.PostAsJsonAsync(
            new Uri("/api/v2/accounts/favorite-account", UriKind.Relative), new { Id = id, IsFavorite = isFavorite });
    }

    private static async Task AValidationErrorIsReturnedOnTheId(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem.Title.Should().Be(ValidationErrorTitle);
        problem.Errors.Should().ContainKey(IdErrors.CodeInvalid);
    }

    private static async Task TheAccountIsNotFound(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Detail.Should().Be(AccountErrors.NotFound.Description);
    }

    private async Task TheCreditCardIsAFavorite(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var client = server.CreateClientFor(_user.IdentityObjectId);
        var account = await client.GetFromJsonAsync<FavoriteAccountJson>(new Uri($"/api/v2/accounts/{_amex}/detail", UriKind.Relative));
        account.Should().Be(new FavoriteAccountJson(_amex, Favorite: true));
    }

    private sealed record FavoriteAccountJson(Guid Id, bool Favorite);
}
