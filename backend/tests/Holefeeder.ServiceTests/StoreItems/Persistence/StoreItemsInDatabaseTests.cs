using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Shared.Core.Search;
using Holefeeder.Service.StoreItems.Core;
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
    public async Task GetAsync_Succeed_WhenTheUserOwnsTheItem()
    {
        var user = await AUser().SavedInDbAsync(database);
        var item = await AStoreItem().SavedInDbAsync(user, database);
        await AStoreItem().SavedInDbAsync(user, database);

        var result = await _repository.GetAsync(AGetStoreItemRequest(user, item.Id), CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(item);
    }

    [Fact]
    public async Task GetAsync_FailsWithNotFound_WhenTheItemBelongsToAnotherUser()
    {
        var owner = await AUser().SavedInDbAsync(database);
        var item = await AStoreItem().SavedInDbAsync(owner, database);
        var user = await AUser().SavedInDbAsync(database);

        var result = await _repository.GetAsync(AGetStoreItemRequest(user, item.Id), CancellationToken.None);

        result.Should().BeFailure().And.WithError(StoreItemErrors.NotFound);
    }

    [Fact]
    public async Task GetAsync_FailsWithNotFound_WhenTheItemDoesNotExist()
    {
        var user = await AUser().SavedInDbAsync(database);

        var result = await _repository.GetAsync(AGetStoreItemRequest(user, AnId()), CancellationToken.None);

        result.Should().BeFailure().And.WithError(StoreItemErrors.NotFound);
    }

    [Fact]
    public async Task GetAsync_Fails_WhenDatabaseError()
    {
        var repo = new StoreItemsInDatabase(database.BadDataSource, new LoggerForTest<StoreItemsInDatabase>(testOutputHelper));

        var result = await repo.GetAsync(AGetStoreItemRequest(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(StoreItemsErrors.ReadFailed);
    }

    [Fact]
    public async Task SaveAsync_InsertsTheItem_WhenItDoesNotExist()
    {
        var user = await AUser().SavedInDbAsync(database);
        var item = AStoreItem();

        var result = await _repository.SaveAsync(user, item, CancellationToken.None);

        result.Should().BeSuccessful();
        (await _repository.GetAsync(AGetStoreItemRequest(user, item.Id), CancellationToken.None))
            .Should().BeSuccessful().And.WithValue(item);
    }

    [Fact]
    public async Task SaveAsync_ReplacesTheItem_WhenTheUserOwnsIt()
    {
        var user = await AUser().SavedInDbAsync(database);
        var item = await AStoreItem().SavedInDbAsync(user, database);
        var other = await AStoreItem().SavedInDbAsync(user, database);
        var modified = item with { Code = ACode(), Data = "new-data" };

        var result = await _repository.SaveAsync(user, modified, CancellationToken.None);

        result.Should().BeSuccessful();
        (await _repository.GetAsync(AGetStoreItemRequest(user, item.Id), CancellationToken.None))
            .Should().BeSuccessful().And.WithValue(modified);
        (await _repository.GetAsync(AGetStoreItemRequest(user, other.Id), CancellationToken.None))
            .Should().BeSuccessful().And.WithValue(other);
    }

    [Fact]
    public async Task SaveAsync_FailsWithNotFound_WhenTheItemBelongsToAnotherUser()
    {
        var owner = await AUser().SavedInDbAsync(database);
        var item = await AStoreItem().SavedInDbAsync(owner, database);
        var user = await AUser().SavedInDbAsync(database);

        var result = await _repository.SaveAsync(user, item with { Data = "new-data" }, CancellationToken.None);

        result.Should().BeFailure().And.WithError(StoreItemErrors.NotFound);
        (await _repository.GetAsync(AGetStoreItemRequest(owner, item.Id), CancellationToken.None))
            .Should().BeSuccessful().And.WithValue(item);
    }

    [Fact]
    public async Task SaveAsync_FailsWithCodeAlreadyExists_WhenTheUserHasTheCode()
    {
        var user = await AUser().SavedInDbAsync(database);
        var existing = await AStoreItem().SavedInDbAsync(user, database);

        var result = await _repository.SaveAsync(user, AStoreItem(existing.Code), CancellationToken.None);

        result.Should().BeFailure().And.WithError(StoreItemErrors.CodeAlreadyExists);
    }

    [Fact]
    public async Task SaveAsync_InsertsTheItem_WhenAnotherUserHasTheCode()
    {
        var owner = await AUser().SavedInDbAsync(database);
        var existing = await AStoreItem().SavedInDbAsync(owner, database);
        var user = await AUser().SavedInDbAsync(database);
        var item = AStoreItem(existing.Code);

        var result = await _repository.SaveAsync(user, item, CancellationToken.None);

        result.Should().BeSuccessful();
        (await _repository.GetAsync(AGetStoreItemRequest(user, item.Id), CancellationToken.None))
            .Should().BeSuccessful().And.WithValue(item);
    }

    [Fact]
    public async Task SaveAsync_Fails_WhenDatabaseError()
    {
        var repo = new StoreItemsInDatabase(database.BadDataSource, new LoggerForTest<StoreItemsInDatabase>(testOutputHelper));

        var result = await repo.SaveAsync(AUser(), AStoreItem(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(StoreItemsErrors.WriteFailed);
    }

    [Fact]
    public void WriteFailed_IsAnUnexpectedError() =>
        StoreItemsErrors.WriteFailed.Should().BeOfType<UnexpectedError>();

    [Fact]
    public async Task GetByIdentityObjectId_Fails_WhenDatabaseError()
    {
        var repo = new StoreItemsInDatabase(database.BadDataSource, new LoggerForTest<StoreItemsInDatabase>(testOutputHelper));

        var result = await repo.SearchAsync(ASearchStoreItemsRequest(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(StoreItemsErrors.ReadFailed);
    }

    [Fact]
    public void ReadFailed_IsAnUnexpectedError() =>
        StoreItemsErrors.ReadFailed.Should().BeOfType<UnexpectedError>();

    [Fact]
    public void ReadFailed_MatchesUsersReadFailed()
    {
        StoreItemsErrors.ReadFailed.Code.Should().Be("store-items-read-failed");
        StoreItemsErrors.ReadFailed.Description.Should().Be("Store items could not be read from the database.");
    }
}
