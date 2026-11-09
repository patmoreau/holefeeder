using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.StoreItems.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Holefeeder.ServiceTests.StoreItems.Api.GetStoreItem;

[Collection(ComponentTestMarker.Name)]
public class GetStoreItemComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private const string ValidationErrorTitle = "One or more validation errors occurred.";

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private StoreItem _item = null!;
    private Uri _location = null!;

    [Fact]
    public Task WhenTheIdIsEmpty() =>
        ScenarioRunner.Create("Getting a store item with an empty id", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user gets the store item with an empty id", () => GettingTheStoreItem(Guid.Empty))
            .Then<HttpResponseMessage>("a validation error is returned on the id", AValidationErrorIsReturnedOnTheId)
            .PlayAsync();

    [Fact]
    public Task WhenTheItemDoesNotExist() =>
        ScenarioRunner.Create("Getting a store item that does not exist", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user gets a store item that does not exist", () => GettingTheStoreItem(Guid.NewGuid()))
            .Then<HttpResponseMessage>("the store item is not found", TheStoreItemIsNotFound)
            .PlayAsync();

    [Fact]
    public Task WhenTheItemBelongsToAnotherUser() =>
        ScenarioRunner.Create("Getting another user's store item", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("another user has an item in the store", AnotherUserHasAnItemInTheStore)
            .When("the user gets the other user's store item", () => GettingTheStoreItem(_item.Id.Value))
            .Then<HttpResponseMessage>("the store item is not found", TheStoreItemIsNotFound)
            .PlayAsync();

    [Fact]
    public Task WhenTheItemExists() =>
        ScenarioRunner.Create("Getting a store item", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has an item in the store", TheUserHasAnItemInTheStore)
            .When("the user gets the store item", () => GettingTheStoreItem(_item.Id.Value))
            .Then<HttpResponseMessage>("the store item is returned", TheStoreItemIsReturned)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsNotRegistered() =>
        ScenarioRunner.Create("Getting a store item as a user who never registered", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .And("another user has an item in the store", AnotherUserHasAnItemInTheStore)
            .When("the user gets the other user's store item", () => GettingTheStoreItem(_item.Id.Value))
            .Then<HttpResponseMessage>("the store item is not found", TheStoreItemIsNotFound)
            .PlayAsync();

    [Fact]
    public Task WhenTheItemWasJustCreated() =>
        ScenarioRunner.Create("Getting a store item from the location of its creation", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user created an item in the store", TheUserCreatedAnItemInTheStore)
            .When("the user gets the store item at the returned location", GettingTheCreatedStoreItem)
            .Then<HttpResponseMessage>("the created store item is returned", TheCreatedStoreItemIsReturned)
            .PlayAsync();

    private async Task ARegisteredUser() =>
        _user = await AUser().SavedInDbAsync(server.Database);

    private void AnUnregisteredUser() => _user = AUser();

    private async Task TheUserHasAnItemInTheStore() =>
        _item = await AStoreItem().SavedInDbAsync(_user, server.Database);

    private async Task AnotherUserHasAnItemInTheStore()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        _item = await AStoreItem().SavedInDbAsync(otherUser, server.Database);
    }

    private async Task TheUserCreatedAnItemInTheStore()
    {
        _item = AStoreItem();
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        using var response = await client.PostAsJsonAsync(
            new Uri("/api/v2/store-items/create-store-item", UriKind.Relative),
            new { Code = _item.Code.Value, _item.Data });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        _location = response.Headers.Location!;
    }

    private async Task<HttpResponseMessage> GettingTheStoreItem(Guid id)
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri($"/api/v2/store-items/{id}", UriKind.Relative));
    }

    private async Task<HttpResponseMessage> GettingTheCreatedStoreItem()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(_location);
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

    private static async Task TheStoreItemIsNotFound(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Detail.Should().Be(StoreItemErrors.NotFound.Description);
    }

    private async Task TheStoreItemIsReturned(Ensure<HttpResponseMessage> response) =>
        (await SuccessfulItemFrom(response)).Should().Be(ToResponse(_item));

    private async Task TheCreatedStoreItemIsReturned(Ensure<HttpResponseMessage> response) =>
        (await SuccessfulItemFrom(response)).Should().BeEquivalentTo(new { Code = _item.Code.Value, _item.Data });

    private static async Task<StoreItemResponse> SuccessfulItemFrom(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        var item = await response.Value.Content.ReadFromJsonAsync<StoreItemResponse>();
        item.Should().NotBeNull();
        return item;
    }

    private static StoreItemResponse ToResponse(StoreItem item) => new(item.Id.Value, item.Code.Value, item.Data);

    private sealed record StoreItemResponse(Guid Id, string Code, string Data);
}
