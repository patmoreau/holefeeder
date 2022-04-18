using System.Text.Json;

using FluentAssertions;

using Holefeeder.Application.Features.StoreItems.Commands;
using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

using TechTalk.SpecFlow.Assist;

namespace Holefeeder.FunctionalTests.Steps;

[Binding]
public sealed class StoreItemsStepDefinitions
{
    private readonly HttpClientDriver _httpClientDriver;
    private readonly DatabaseDriver _databaseDriver;

    public StoreItemsStepDefinitions(HttpClientDriver httpClientDriver, DatabaseDriver databaseDriver)
    {
        _httpClientDriver = httpClientDriver;
        _databaseDriver = databaseDriver;
    }

    [Given(@"the following items")]
    public async Task GivenTheFollowingItems(Table table)
    {
        await _databaseDriver.AddStoreItemsToDatabase(table);
    }

    [When("I try to (GetStoreItems?)")]
    public Task WhenITryToFetchStoreItems(ApiResources apiResource) => _httpClientDriver.SendGetRequest(apiResource);

    [When("I try to (CreateStoreItem|ModifyStoreItem)")]
    public Task WhenITryToCreateStoreItems(ApiResources apiResource) => _httpClientDriver.SendPostRequest(apiResource);

    [When("I try to (GetStoreItems?) with an invalid request")]
    public Task WhenITryToFetchStoreItemsWithAnInvalidRequest(ApiResources apiResource) =>
        _httpClientDriver.SendGetRequest(apiResource, "limit=-1");

    [When(@"I try to (GetStoreItems?|CreateStoreItem) with an empty id")]
    public Task WhenITryToFetchStoreItemWithAnEmptyId(ApiResources apiResource) =>
        _httpClientDriver.SendGetRequest(apiResource, new object?[] {Guid.Empty.ToString()});

    [When(@"I try to (GetStoreItem) with id '(.*)'")]
    public Task WhenITryToFetchStoreItemWithId(ApiResources apiResource, string id)
    {
        return _httpClientDriver.SendGetRequest(apiResource, new object?[] {id});
    }

    [When(@"I try to (CreateStoreItem) with code '(.*)' and data '(.*)'")]
    public async Task WhenITryToCreateStoreItemWithCodeAndData(ApiResources apiResource, string code, string data)
    {
        await _httpClientDriver.SendPostRequest(apiResource, $"{{\"code\": \"{code}\", \"data\": \"{data}\"}}");
    }

    [When(@"I try to (ModifyStoreItem) with id '(.*)' and data '(.*)'")]
    public async Task WhenITryToModifyStoreItemWithIdAndData(ApiResources apiResource, string id, string data)
    {
        await _httpClientDriver.SendPostRequest(apiResource, $"{{\"id\": \"{id}\", \"data\": \"{data}\"}}");
    }

    [Then("I get my expected items")]
    public async Task ThenIGetMyExpectedItems(Table table)
    {
        var result = await _httpClientDriver.DeserializeContent<StoreItemViewModel[]>();

        result.Should().NotBeNull().And.ContainInOrder(ToExpectedList(table));
    }

    [Then(@"I get my expected item")]
    public async Task ThenIGetMyExpectedItem(Table table)
    {
        var result = await _httpClientDriver.DeserializeContent<StoreItemViewModel>();

        result.Should().NotBeNull().And.BeEquivalentTo(ToExpectedResult(table));
    }

    [Then(@"I get the route of the new resource in the header")]
    public void ThenIGetTheRouteOfTheNewResourceInTheHeader()
    {
        var headers = _httpClientDriver.ResponseMessage!.Headers;

        headers.Should().ContainKey("Location");//.And.ContainValue(new[] {"http://localhost/api/v2/store-items/df865211-60cc-4dcd-ad59-865352e446df"});
    }

    private static StoreItemViewModel ToExpectedResult(Table items) => items.CreateInstance<StoreItemViewModel>();

    private static IEnumerable<StoreItemViewModel> ToExpectedList(Table items)
    {
        var rows = items.CreateSet<StoreItemViewModel>();

        foreach (var row in rows)
        {
            yield return row;
        }
    }
}
