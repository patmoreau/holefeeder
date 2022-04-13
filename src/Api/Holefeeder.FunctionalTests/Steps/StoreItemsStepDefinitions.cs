using System.Text.Json;

using FluentAssertions;

using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

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

    [When("I try to fetch (StoreItems?)")]
    public Task WhenITryToFetchStoreItems(ApiResources apiResource) => _httpClientDriver.SendGetRequest(apiResource);

    [When("I try to fetch (StoreItems?) with an invalid request")]
    public Task WhenITryToFetchStoreItemsWithAnInvalidRequest(ApiResources apiResource) =>
        _httpClientDriver.SendGetRequest(apiResource, "limit=-1");

    [Then("I get my expected items")]
    public async Task ThenIGetMyExpectedItems()
    {
        var result = await _httpClientDriver.DeserializeContent<StoreItemViewModel[]>();

        result.Should().NotBeNull();
    }

    [When(@"I try to fetch (StoreItems?) with an empty id")]
    public Task WhenITryToFetchStoreItemWithAnEmptyId(ApiResources apiResource) =>
        _httpClientDriver.SendGetRequest(apiResource, new object?[] {Guid.Empty.ToString()});

    [When(@"I try to fetch (StoreItem) with id '(.*)'")]
    public Task WhenITryToFetchStoreItemWithId(ApiResources apiResource, string id)
    {
        return _httpClientDriver.SendGetRequest(apiResource, new object?[] {id});
    }

    [Given(@"the following items")]
    public async Task GivenTheFollowingItems(Table table)
    {
        await _databaseDriver.AddToDatabase(table);
    }

    [Then(@"I get my expected item")]
    public async Task ThenIGetMyExpectedItem(Table table)
    {
        var result = await _httpClientDriver.DeserializeContent<StoreItemViewModel>();

        result.Should().NotBeNull();
    }
}
