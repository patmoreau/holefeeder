using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Shared.Core.Search;
using Holefeeder.Service.StoreItems.Core;
using Holefeeder.Service.StoreItems.Core.SearchStoreItems;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Holefeeder.ServiceTests.StoreItems.Api.SearchStoreItems;

[Collection(ComponentTestMarker.Name)]
public class SearchStoreItemsComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private const string ValidationErrorTitle = "One or more validation errors occurred.";
    private const string SettingsData = """{"effectiveDate":"2026-01-01","frequency":1}""";

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private ReadOnlyCollection<StoreItem> _items = [];
    private StoreItem _settings = null!;

    [Fact]
    public Task WhenTheRequestIsInvalid() =>
        ScenarioRunner.Create("Getting store items with a negative offset", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user gets the store items from offset -1",
                () => GettingStoreItems("?offset=-1&limit=10"))
            .Then<HttpResponseMessage>("a validation error is returned on the offset",
                response => AValidationErrorIsReturnedOn(response, OffsetErrors.CodeInvalid))
            .PlayAsync();

    [Fact]
    public Task WhenTheFilterOperatorIsNotSupported() =>
        ScenarioRunner.Create("Getting store items with an unsupported filter operator", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user gets the store items filtered on code:lt:settings",
                () => GettingStoreItems("?filter=code:lt:settings"))
            .Then<HttpResponseMessage>("a validation error is returned on the filter operator",
                response => AValidationErrorIsReturnedOn(response,
                    SearchStoreItemsRequestFilterByErrors.CodeOperatorNotSupported))
            .PlayAsync();

    [Fact]
    public Task WhenSortedByCodeDescending() =>
        ScenarioRunner.Create("Getting store items sorted by code descending", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has several items in the store", TheUserHasItemsInTheStore)
            .When("the user gets the store items sorted by -code",
                () => GettingStoreItems("?offset=0&limit=10&sort=-code"))
            .Then<HttpResponseMessage>("every item is returned in descending code order",
                EveryItemIsReturnedInDescendingCodeOrder)
            .PlayAsync();

    [Fact]
    public Task WhenOtherUsersHaveItems() =>
        ScenarioRunner.Create("Getting store items while other users have items", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has several items in the store", TheUserHasItemsInTheStore)
            .And("another user has items in the store", AnotherUserHasItemsInTheStore)
            .When("the user gets the store items", () => GettingStoreItems(string.Empty))
            .Then<HttpResponseMessage>("only the user's items are returned and counted",
                OnlyTheUsersItemsAreReturnedAndCounted)
            .PlayAsync();

    [Fact]
    public Task WhenFilteredByCode() =>
        ScenarioRunner.Create("Getting the settings store item by its code", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has several items in the store", TheUserHasItemsInTheStore)
            .And("the user has a settings item in the store", TheUserHasASettingsItemInTheStore)
            .When("the user gets the store items filtered on code:eq:settings",
                () => GettingStoreItems("?filter=code:eq:settings"))
            .Then<HttpResponseMessage>("only the settings item is returned", OnlyTheSettingsItemIsReturned)
            .PlayAsync();

    [Fact]
    public Task WhenPaged() =>
        ScenarioRunner.Create("Getting a page of store items", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has five items in the store", TheUserHasFiveItemsInTheStore)
            .When("the user gets two items sorted by code from offset 1",
                () => GettingStoreItems("?offset=1&limit=2&sort=code"))
            .Then<HttpResponseMessage>("the second and third items are returned out of five",
                TheSecondAndThirdItemsAreReturnedOutOfFive)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsNotRegistered() =>
        ScenarioRunner.Create("Getting store items as a user who never registered", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .And("another user has items in the store", AnotherUserHasItemsInTheStore)
            .When("the user gets the store items", () => GettingStoreItems(string.Empty))
            .Then<HttpResponseMessage>("no item is returned", NoItemIsReturned)
            .PlayAsync();

    private async Task ARegisteredUser() =>
        _user = await AUser().SavedInDbAsync(server.Database);

    private void AnUnregisteredUser() => _user = AUser();

    private async Task TheUserHasItemsInTheStore() =>
        _items = [
            await AStoreItem().SavedInDbAsync(_user, server.Database),
            await AStoreItem().SavedInDbAsync(_user, server.Database),
            await AStoreItem().SavedInDbAsync(_user, server.Database)
        ];

    private async Task TheUserHasFiveItemsInTheStore() =>
        _items = [
            await AStoreItem().SavedInDbAsync(_user, server.Database),
            await AStoreItem().SavedInDbAsync(_user, server.Database),
            await AStoreItem().SavedInDbAsync(_user, server.Database),
            await AStoreItem().SavedInDbAsync(_user, server.Database),
            await AStoreItem().SavedInDbAsync(_user, server.Database)
        ];

    private async Task TheUserHasASettingsItemInTheStore() =>
        _settings = await AStoreItem(ACode("settings"), SettingsData).SavedInDbAsync(_user, server.Database);

    private async Task AnotherUserHasItemsInTheStore()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        await AStoreItem().SavedInDbAsync(otherUser, server.Database);
        await AStoreItem().SavedInDbAsync(otherUser, server.Database);
        await AStoreItem().SavedInDbAsync(otherUser, server.Database);
        await AStoreItem(ACode("settings"), SettingsData).SavedInDbAsync(otherUser, server.Database);
    }

    private async Task<HttpResponseMessage> GettingStoreItems(string query)
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri($"/api/v2/store-items{query}", UriKind.Relative));
    }

    private static async Task AProblemWithoutDatabaseDetailsIsReturned(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Detail.Should().Be("Store items could not be read.");
    }

    private async Task EveryItemIsReturnedInDescendingCodeOrder(Ensure<HttpResponseMessage> response)
    {
        var items = await SuccessfulItemsFrom(response);
        items.Should().BeEquivalentTo(_items.Select(ToResponse))
            .And.BeInDescendingOrder(item => item.Code, StringComparer.Ordinal);
    }

    private async Task OnlyTheUsersItemsAreReturnedAndCounted(Ensure<HttpResponseMessage> response)
    {
        var items = await SuccessfulItemsFrom(response);
        items.Should().BeEquivalentTo(_items.Select(ToResponse));
        TotalCountOf(response).Should().Be(_items.Count);
    }

    private async Task OnlyTheSettingsItemIsReturned(Ensure<HttpResponseMessage> response)
    {
        var items = await SuccessfulItemsFrom(response);
        items.Should().ContainSingle().Which.Should().BeEquivalentTo(ToResponse(_settings));
    }

    private async Task TheSecondAndThirdItemsAreReturnedOutOfFive(Ensure<HttpResponseMessage> response)
    {
        var items = await SuccessfulItemsFrom(response);
        var expected = _items.OrderBy(item => item.Code.Value, StringComparer.Ordinal).Skip(1).Take(2).Select(ToResponse);
        items.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
        TotalCountOf(response).Should().Be(5);
    }

    private static async Task NoItemIsReturned(Ensure<HttpResponseMessage> response)
    {
        var items = await SuccessfulItemsFrom(response);
        items.Should().BeEmpty();
        TotalCountOf(response).Should().Be(0);
    }

    private static async Task AValidationErrorIsReturnedOn(Ensure<HttpResponseMessage> response,
        string invalidParameter)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem.Title.Should().Be(ValidationErrorTitle);
        problem.Errors.Should().ContainKey(invalidParameter);
    }

    private static async Task<IReadOnlyList<StoreItemResponse>> SuccessfulItemsFrom(
        Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Value.Content.ReadFromJsonAsync<List<StoreItemResponse>>();
        return items ?? [];
    }

    private static int TotalCountOf(Ensure<HttpResponseMessage> response) =>
        int.Parse(response.Value.Headers.GetValues("X-Total-Count").Single(),
            System.Globalization.CultureInfo.InvariantCulture);

    private static StoreItemResponse ToResponse(StoreItem item) => new(item.Id.Value, item.Code.Value, item.Data);

    private sealed record StoreItemResponse(Guid Id, string Code, string Data);
}
