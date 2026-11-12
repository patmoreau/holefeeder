using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

namespace Holefeeder.ServiceTests.Users.Api.GetCurrentUser;

[Collection(ComponentTestMarker.Name)]
public class GetCurrentUserComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;

    [Fact]
    public Task WhenTheUserIsRegistered() =>
        ScenarioRunner.Create("Getting the current user when registered", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user gets the current user", GettingTheCurrentUser)
            .Then<HttpResponseMessage>("the user's id is returned", TheUsersIdIsReturned)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsNotRegistered() =>
        ScenarioRunner.Create("Getting the current user when not registered", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .When("the user gets the current user", GettingTheCurrentUser)
            .Then<HttpResponseMessage>("the user is not found", TheUserIsNotFound)
            .PlayAsync();

    private async Task ARegisteredUser() =>
        _user = await AUser().SavedInDbAsync(server.Database);

    private void AnUnregisteredUser() => _user = AUser();

    private async Task<HttpResponseMessage> GettingTheCurrentUser()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri("/api/v2/users/me", UriKind.Relative));
    }

    private async Task TheUsersIdIsReturned(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Value.Content.ReadFromJsonAsync<CurrentUserResponse>();
        body.Should().Be(new CurrentUserResponse(_user.Id.Value));
    }

    private static void TheUserIsNotFound(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private sealed record CurrentUserResponse(Guid Id);
}
