using Holefeeder.Service.Shared.Core.Search;
using Holefeeder.Service.StoreItems.Core.SearchStoreItems;
using Holefeeder.Service.StoreItems.Persistence;
using Holefeeder.ServiceTests.StoreItems.TestKit;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.StoreItems.Persistence;

[Collection(DatabaseTestMarker.Name)]
public class StoreItemsInDatabaseTests(DatabaseForTest database, ITestOutputHelper testOutputHelper)
{
    private readonly StoreItemsInDatabase _repository = new(database.DataSource, new LoggerForTest<StoreItemsInDatabase>(testOutputHelper));

    [Fact]
    public async Task SearchAsync_Succeed_WhenFound()
    {
        var user = await AUser().SavedInDbAsync(database);
        var item1 = await AStoreItem(ACode("code-1")).SavedInDbAsync(user, database);
        await AStoreItem(ACode("code-2")).SavedInDbAsync(user, database);
        await AStoreItem(ACode("code-3")).SavedInDbAsync(user, database);
        var request = ASearchStoreItemsRequest(user: user, filterBy: ASearchStoreItemsRequestFilterBy(ACode("code-1")));
        var result = await _repository.SearchAsync(request, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo(new StoreItemsPage(1, [item1]));
    }

    [Fact]
    public async Task SearchAsync_Succeed_WhenNoneFound()
    {
        var user = await AUser().SavedInDbAsync(database);
        await AStoreItem(ACode("code-1")).SavedInDbAsync(user, database);
        await AStoreItem(ACode("code-2")).SavedInDbAsync(user, database);
        await AStoreItem(ACode("code-3")).SavedInDbAsync(user, database);
        var request = ASearchStoreItemsRequest(user: user, filterBy: ASearchStoreItemsRequestFilterBy(ACode("code-4")));
        var result = await _repository.SearchAsync(request, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo(new StoreItemsPage(0, []));
    }

    [Fact]
    public async Task SearchAsync_Succeed_WhenNotEqual()
    {
        var user = await AUser().SavedInDbAsync(database);
        var item1 = await AStoreItem(ACode("code-1")).SavedInDbAsync(user, database);
        var item2 = await AStoreItem(ACode("code-2")).SavedInDbAsync(user, database);
        var item3 = await AStoreItem(ACode("code-3")).SavedInDbAsync(user, database);
        var filter = ASearchStoreItemsRequestFilterBy(ACode("code-4"), FilterByOperator.NotEqual);
        var request = ASearchStoreItemsRequest(user: user, filterBy: filter);
        var result = await _repository.SearchAsync(request, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo(new StoreItemsPage(3, [item1, item2, item3]));
    }

    [Fact]
    public async Task SearchAsync_SucceedInDescendingOrder()
    {
        var user = await AUser().SavedInDbAsync(database);
        var item1 = await AStoreItem(ACode("code-1")).SavedInDbAsync(user, database);
        var item2 = await AStoreItem(ACode("code-2")).SavedInDbAsync(user, database);
        var item3 = await AStoreItem(ACode("code-3")).SavedInDbAsync(user, database);
        var filter = ASearchStoreItemsRequestFilterBy(ACode("code-4"), FilterByOperator.NotEqual);
        var sort = ASearchStoreItemsRequestSortBy("code", SortByDirection.Descending);
        var request = ASearchStoreItemsRequest(user: user, filterBy: filter, sortBy: sort);
        var result = await _repository.SearchAsync(request, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo(new StoreItemsPage(3, [item3, item2, item1]));
    }

    [Fact]
    public async Task GetByIdentityObjectId_Fails_WhenDatabaseError()
    {
        var repo = new StoreItemsInDatabase(database.BadDataSource, new LoggerForTest<StoreItemsInDatabase>(testOutputHelper));

        var result = await repo.SearchAsync(ASearchStoreItemsRequest(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(StoreItemsErrors.DatabaseFailed);
    }
}
