using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core.Search;
using Holefeeder.Service.StoreItems.Core.SearchStoreItems;
using Holefeeder.ServiceTests.StoreItems.TestKit;

namespace Holefeeder.ServiceTests.StoreItems.Core.SearchStoreItems;

public class SearchStoreItemsUseCaseTests
{
    private readonly StoreItemsInMemory _storeItems = new();
    private readonly SearchStoreItemsUseCase _useCase;

    public SearchStoreItemsUseCaseTests() => _useCase = new SearchStoreItemsUseCase(_storeItems);

    [Fact]
    public async Task HandleAsync_ReturnsMatchingItem_WhenCodeIsEqual()
    {
        var user = AUser();
        var settings = AStoreItem(ACode("settings"));
        _storeItems.Add(user, settings);
        _storeItems.Add(user, AStoreItem(ACode("preferences")));
        var request = ASearchStoreItemsRequest(user: user, filterBy: ASearchStoreItemsRequestFilterBy(ACode("settings")));

        var result = await _useCase.HandleAsync(request, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo(new StoreItemsPage(1, [settings]));
    }

    [Fact]
    public async Task HandleAsync_ReturnsEmptyPage_WhenNothingMatches()
    {
        var user = AUser();
        _storeItems.Add(user, AStoreItem(ACode("settings")));
        var request = ASearchStoreItemsRequest(user: user, filterBy: ASearchStoreItemsRequestFilterBy(ACode("unknown")));

        var result = await _useCase.HandleAsync(request, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo(new StoreItemsPage(0, []));
    }

    [Fact]
    public async Task HandleAsync_ExcludesItemsOfOtherUsers()
    {
        var user = AUser();
        var mine = AStoreItem(ACode("settings"));
        _storeItems.Add(user, mine);
        _storeItems.Add(AUser(), AStoreItem(ACode("settings")));
        var request = ASearchStoreItemsRequest(user: user, filterBy: ASearchStoreItemsRequestFilterBy(ACode("settings")));

        var result = await _useCase.HandleAsync(request, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo(new StoreItemsPage(1, [mine]));
    }

    [Fact]
    public async Task HandleAsync_ReturnsAllOtherItemsInAscendingOrder_WhenCodeIsNotEqual()
    {
        var user = AUser();
        var itemB = AStoreItem(ACode("code-b"));
        var itemA = AStoreItem(ACode("code-a"));
        _storeItems.Add(user, itemB);
        _storeItems.Add(user, itemA);
        _storeItems.Add(user, AStoreItem(ACode("code-c")));
        var filter = ASearchStoreItemsRequestFilterBy(ACode("code-c"), FilterByOperator.NotEqual);
        var request = ASearchStoreItemsRequest(user: user, filterBy: filter);

        var result = await _useCase.HandleAsync(request, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo(new StoreItemsPage(2, [itemA, itemB]), options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task HandleAsync_ReturnsItemsInDescendingOrder_WhenSortIsDescending()
    {
        var user = AUser();
        var itemA = AStoreItem(ACode("code-a"));
        var itemC = AStoreItem(ACode("code-c"));
        var itemB = AStoreItem(ACode("code-b"));
        _storeItems.Add(user, itemA);
        _storeItems.Add(user, itemC);
        _storeItems.Add(user, itemB);
        var filter = ASearchStoreItemsRequestFilterBy(ACode("none"), FilterByOperator.NotEqual);
        var sort = ASearchStoreItemsRequestSortBy("code", SortByDirection.Descending);
        var request = ASearchStoreItemsRequest(user: user, filterBy: filter, sortBy: sort);

        var result = await _useCase.HandleAsync(request, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo(new StoreItemsPage(3, [itemC, itemB, itemA]), options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task HandleAsync_ReturnsRequestedPageWithFullTotal_WhenPaging()
    {
        var user = AUser();
        var itemB = AStoreItem(ACode("code-b"));
        var itemC = AStoreItem(ACode("code-c"));
        _storeItems.Add(user, AStoreItem(ACode("code-a")));
        _storeItems.Add(user, itemB);
        _storeItems.Add(user, itemC);
        _storeItems.Add(user, AStoreItem(ACode("code-d")));
        var filter = ASearchStoreItemsRequestFilterBy(ACode("none"), FilterByOperator.NotEqual);
        var request = ASearchStoreItemsRequest(user: user, offset: Offset.Valid(1), limit: Limit.Valid(2), filterBy: filter);

        var result = await _useCase.HandleAsync(request, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo(new StoreItemsPage(4, [itemB, itemC]), options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task HandleAsync_ReturnsEmptyItemsWithFullTotal_WhenOffsetIsPastTheEnd()
    {
        var user = AUser();
        _storeItems.Add(user, AStoreItem(ACode("code-a")));
        _storeItems.Add(user, AStoreItem(ACode("code-b")));
        var filter = ASearchStoreItemsRequestFilterBy(ACode("none"), FilterByOperator.NotEqual);
        var request = ASearchStoreItemsRequest(user: user, offset: Offset.Valid(5), filterBy: filter);

        var result = await _useCase.HandleAsync(request, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo(new StoreItemsPage(2, []));
    }

    [Fact]
    public async Task HandleAsync_ReturnsStoreError_WhenStoreFails()
    {
        var error = new ResultError("store-failed", "Store failed.");
        _storeItems.FailWith(error);

        var result = await _useCase.HandleAsync(ASearchStoreItemsRequest(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(error);
    }
}
