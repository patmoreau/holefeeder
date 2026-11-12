using System.Net;
using System.Net.Http.Json;

using Dapper;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Users.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

namespace Holefeeder.ServiceTests.Users.Api.RegisterUser;

[Collection(ComponentTestMarker.Name)]
public class RegisterUserComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _registeredId;

    [Fact]
    public Task WhenTheUserIsNotRegisteredYet() =>
        ScenarioRunner.Create("Registering a caller who is not registered yet", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .When("the user registers", Registering)
            .Then<HttpResponseMessage>("the user owns their identity", TheUserOwnsTheirIdentity)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserRegistersThenGetsTheCurrentUser() =>
        ScenarioRunner.Create("Registering makes the caller known", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .And("the user registered", TheUserRegistered)
            .When("the user gets the current user", GettingTheCurrentUser)
            .Then<HttpResponseMessage>("the registered id is returned", TheRegisteredIdIsReturned)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserRegistersTheTransferCategoriesAreSeeded() =>
        ScenarioRunner.Create("Registering seeds the transfer categories", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .When("the user registers", Registering)
            .Then<HttpResponseMessage>("the user owns the transfer categories", TheUserOwnsTheTransferCategories)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsAlreadyRegistered() =>
        ScenarioRunner.Create("Registering a caller who is already registered", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user registers", Registering)
            .Then<HttpResponseMessage>("a bad request is returned", ABadRequestIsReturned)
            .PlayAsync();

    private async Task ARegisteredUser() =>
        _user = await AUser().SavedInDbAsync(server.Database);

    private void AnUnregisteredUser() => _user = AUser();

    private async Task TheUserRegistered()
    {
        using var response = await Registering();
        _registeredId = (await IdFromAsync(response)).Id;
    }

    private async Task<HttpResponseMessage> Registering()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.PostAsync(new Uri("/api/v2/users/register", UriKind.Relative), null);
    }

    private async Task<HttpResponseMessage> GettingTheCurrentUser()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri("/api/v2/users/me", UriKind.Relative));
    }

    private async Task TheUserOwnsTheirIdentity(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        var id = (await IdFromAsync(response.Value)).Id;
        id.Should().NotBeEmpty();

        await using var connection = server.Database.DataSource.CreateConnection();
        var identities = await connection.QueryAsync<string>(
            "SELECT i.identity_object_id FROM users u JOIN user_identities i ON i.user_id = u.id WHERE u.id = @Id AND NOT u.inactive",
            new { Id = id });
        identities.Should().ContainSingle().Which.Should().Be(_user.IdentityObjectId.Value);
    }

    private async Task TheRegisteredIdIsReturned(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        (await IdFromAsync(response.Value)).Id.Should().Be(_registeredId);
    }

    private async Task TheUserOwnsTheTransferCategories(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        var id = (await IdFromAsync(response.Value)).Id;

        await using var connection = server.Database.DataSource.CreateConnection();
        var categories = await connection.QueryAsync<CategoryRow>(
            "SELECT type, name, budget_amount, favorite, system, inactive FROM categories WHERE user_id = @Id",
            new { Id = id });
        categories.Should().BeEquivalentTo(
            [
                new CategoryRow("Expense", SystemCategory.TransferOutName, 0m, false, true, false),
                new CategoryRow("Gain", SystemCategory.TransferInName, 0m, false, true, false)
            ]);
    }

    private static void ABadRequestIsReturned(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private static async Task<IdResponse> IdFromAsync(HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body.Should().NotBeNull();
        return body;
    }

    private sealed record IdResponse(Guid Id);

    private sealed record CategoryRow(string Type, string Name, decimal BudgetAmount, bool Favorite, bool System, bool Inactive);
}
