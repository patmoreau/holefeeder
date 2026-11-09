using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.StoreItems.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Holefeeder.ServiceTests.StoreItems.Api.CreateStoreItem;

[Collection(ComponentTestMarker.Name)]
public class CreateStoreItemComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private const string ValidationErrorTitle = "One or more validation errors occurred.";
    private const string NewData = "{\"theme\":\"dark\"}";

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private StoreItem _existing = null!;
    private Code _code = null!;

    [Fact]
    public Task WhenTheCodeIsEmpty() =>
        ScenarioRunner.Create("Creating a store item with an empty code", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user creates a store item with an empty code", () => CreatingTheStoreItem(string.Empty, NewData))
            .Then<HttpResponseMessage>("a validation error is returned on the code",
                response => AValidationErrorIsReturnedOn(response, CodeErrors.CodeInvalid))
            .PlayAsync();

    [Fact]
    public Task WhenTheDataIsEmpty() =>
        ScenarioRunner.Create("Creating a store item with empty data", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user creates a store item with empty data", () => CreatingTheStoreItem(ACode().Value, string.Empty))
            .Then<HttpResponseMessage>("a validation error is returned on the data",
                response => AValidationErrorIsReturnedOn(response, DataErrors.CodeInvalid))
            .PlayAsync();

    [Fact]
    public Task WhenTheUserAlreadyHasTheCode() =>
        ScenarioRunner.Create("Creating a store item with a code the user already has", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has an item in the store", TheUserHasAnItemInTheStore)
            .When("the user creates a store item with the same code", () => CreatingTheStoreItem(_existing.Code.Value, NewData))
            .Then<HttpResponseMessage>("the code is reported as already existing",
                response => AProblemIsReturned(response, StoreItemErrors.CodeAlreadyExists.Description))
            .PlayAsync();

    [Fact]
    public Task WhenAnotherUserHasTheCode() =>
        ScenarioRunner.Create("Creating a store item with a code another user has", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("another user has an item in the store", AnotherUserHasAnItemInTheStore)
            .When("the user creates a store item with the same code", () => CreatingTheStoreItem(_existing.Code.Value, NewData))
            .Then<HttpResponseMessage>("the store item is created", TheStoreItemIsCreated)
            .PlayAsync();

    [Fact]
    public Task WhenTheCodeIsNew() =>
        ScenarioRunner.Create("Creating a store item", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("a code the user does not have", ANewCode)
            .When("the user creates the store item", () => CreatingTheStoreItem(_code.Value, NewData))
            .Then<HttpResponseMessage>("the store item is created", TheStoreItemIsCreated)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsNotRegistered() =>
        ScenarioRunner.Create("Creating a store item as a user who never registered", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .When("the user creates a store item", () => CreatingTheStoreItem(ACode().Value, NewData))
            .Then<HttpResponseMessage>("the user is reported as not registered",
                response => AProblemIsReturned(response, StoreItemErrors.UserNotRegistered.Description))
            .PlayAsync();

    private async Task ARegisteredUser() =>
        _user = await AUser().SavedInDbAsync(server.Database);

    private void AnUnregisteredUser() => _user = AUser();

    private void ANewCode() => _code = ACode();

    private async Task TheUserHasAnItemInTheStore() =>
        _existing = await AStoreItem().SavedInDbAsync(_user, server.Database);

    private async Task AnotherUserHasAnItemInTheStore()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        _existing = await AStoreItem().SavedInDbAsync(otherUser, server.Database);
        _code = _existing.Code;
    }

    private async Task<HttpResponseMessage> CreatingTheStoreItem(string code, string data)
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.PostAsJsonAsync(
            new Uri("/api/v2/store-items/create-store-item", UriKind.Relative),
            new { Code = code, Data = data });
    }

    private static async Task AValidationErrorIsReturnedOn(Ensure<HttpResponseMessage> response, string errorCode)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem.Title.Should().Be(ValidationErrorTitle);
        problem.Errors.Should().ContainKey(errorCode);
    }

    private static async Task AProblemIsReturned(Ensure<HttpResponseMessage> response, string detail)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Detail.Should().Be(detail);
    }

    private async Task TheStoreItemIsCreated(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Value.Content.ReadFromJsonAsync<CreatedResponse>();
        created.Should().NotBeNull();
        response.Value.Headers.Location.Should().NotBeNull();
        response.Value.Headers.Location!.ToString().Should().EndWith($"/api/v2/store-items/{created.Id}");

        using var client = server.CreateClientFor(_user.IdentityObjectId);
        var item = await client.GetFromJsonAsync<StoreItemResponse>(response.Value.Headers.Location);
        item.Should().Be(new StoreItemResponse(created.Id, _code.Value, NewData));
    }

    private sealed record CreatedResponse(Guid Id);

    private sealed record StoreItemResponse(Guid Id, string Code, string Data);
}
