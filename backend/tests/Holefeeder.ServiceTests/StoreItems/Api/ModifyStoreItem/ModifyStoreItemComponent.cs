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

namespace Holefeeder.ServiceTests.StoreItems.Api.ModifyStoreItem;

[Collection(ComponentTestMarker.Name)]
public class ModifyStoreItemComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private const string ValidationErrorTitle = "One or more validation errors occurred.";
    private const string NewData = "{\"theme\":\"dark\"}";

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private User _owner = null!;
    private StoreItem _item = null!;

    [Fact]
    public Task WhenTheIdIsEmpty() =>
        ScenarioRunner.Create("Modifying a store item with an empty id", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user modifies the store item with an empty id", () => ModifyingTheStoreItem(Guid.Empty, NewData))
            .Then<HttpResponseMessage>("a validation error is returned on the id",
                response => AValidationErrorIsReturnedOn(response, IdErrors.CodeInvalid))
            .PlayAsync();

    [Fact]
    public Task WhenTheDataIsEmpty() =>
        ScenarioRunner.Create("Modifying a store item with empty data", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has an item in the store", TheUserHasAnItemInTheStore)
            .When("the user modifies the store item with empty data", () => ModifyingTheStoreItem(_item.Id.Value, string.Empty))
            .Then<HttpResponseMessage>("a validation error is returned on the data",
                response => AValidationErrorIsReturnedOn(response, DataErrors.CodeInvalid))
            .PlayAsync();

    [Fact]
    public Task WhenTheItemDoesNotExist() =>
        ScenarioRunner.Create("Modifying a store item that does not exist", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user modifies a store item that does not exist", () => ModifyingTheStoreItem(Guid.NewGuid(), NewData))
            .Then<HttpResponseMessage>("the store item is not found", TheStoreItemIsNotFound)
            .PlayAsync();

    [Fact]
    public Task WhenTheItemBelongsToAnotherUser() =>
        ScenarioRunner.Create("Modifying another user's store item", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("another user has an item in the store", AnotherUserHasAnItemInTheStore)
            .When("the user modifies the other user's store item", () => ModifyingTheStoreItem(_item.Id.Value, NewData))
            .Then<HttpResponseMessage>("the store item is not found", TheStoreItemIsNotFound)
            .And("the other user's store item is unchanged", TheOtherUsersStoreItemIsUnchanged)
            .PlayAsync();

    [Fact]
    public Task WhenTheItemExists() =>
        ScenarioRunner.Create("Modifying a store item", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has an item in the store", TheUserHasAnItemInTheStore)
            .When("the user modifies the store item", () => ModifyingTheStoreItem(_item.Id.Value, NewData))
            .Then<HttpResponseMessage>("no content is returned", NoContentIsReturned)
            .And("the store item has the new data", TheStoreItemHasTheNewData)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsNotRegistered() =>
        ScenarioRunner.Create("Modifying a store item as a user who never registered", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .And("another user has an item in the store", AnotherUserHasAnItemInTheStore)
            .When("the user modifies the other user's store item", () => ModifyingTheStoreItem(_item.Id.Value, NewData))
            .Then<HttpResponseMessage>("the store item is not found", TheStoreItemIsNotFound)
            .And("the other user's store item is unchanged", TheOtherUsersStoreItemIsUnchanged)
            .PlayAsync();

    private async Task ARegisteredUser() =>
        _user = await AUser().SavedInDbAsync(server.Database);

    private void AnUnregisteredUser() => _user = AUser();

    private async Task TheUserHasAnItemInTheStore() =>
        _item = await AStoreItem().SavedInDbAsync(_user, server.Database);

    private async Task AnotherUserHasAnItemInTheStore()
    {
        _owner = await AUser().SavedInDbAsync(server.Database);
        _item = await AStoreItem().SavedInDbAsync(_owner, server.Database);
    }

    private async Task<HttpResponseMessage> ModifyingTheStoreItem(Guid id, string data)
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.PostAsJsonAsync(
            new Uri("/api/v2/store-items/modify-store-item", UriKind.Relative),
            new { Id = id, Data = data });
    }

    private async Task<StoreItemResponse?> GettingTheStoreItemAs(User user)
    {
        using var client = server.CreateClientFor(user.IdentityObjectId);
        return await client.GetFromJsonAsync<StoreItemResponse>(
            new Uri($"/api/v2/store-items/{_item.Id.Value}", UriKind.Relative));
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

    private static async Task TheStoreItemIsNotFound(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Detail.Should().Be(StoreItemErrors.NotFound.Description);
    }

    private static void NoContentIsReturned(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task TheStoreItemHasTheNewData() =>
        (await GettingTheStoreItemAs(_user)).Should().Be(ToResponse(_item with { Data = NewData }));

    private async Task TheOtherUsersStoreItemIsUnchanged() =>
        (await GettingTheStoreItemAs(_owner)).Should().Be(ToResponse(_item));

    private static StoreItemResponse ToResponse(StoreItem item) => new(item.Id.Value, item.Code.Value, item.Data);

    private sealed record StoreItemResponse(Guid Id, string Code, string Data);
}
