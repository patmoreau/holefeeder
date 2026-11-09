using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.StoreItems.Core;
using Holefeeder.Service.StoreItems.Core.ModifyStoreItem;
using Holefeeder.ServiceTests.StoreItems.TestKit;

namespace Holefeeder.ServiceTests.StoreItems.Core.ModifyStoreItem;

public class ModifyStoreItemUseCaseTests
{
    private readonly StoreItemsInMemory _storeItems = new();
    private readonly ModifyStoreItemUseCase _useCase;

    public ModifyStoreItemUseCaseTests() => _useCase = new ModifyStoreItemUseCase(_storeItems);

    [Fact]
    public async Task HandleAsync_ReplacesTheData_WhenTheUserOwnsTheItem()
    {
        var user = AUser();
        var item = AStoreItem();
        _storeItems.Add(user, item);

        var result = await _useCase.HandleAsync(
            AModifyStoreItemRequest(user, item.Id, Data.Valid("new-data")), CancellationToken.None);

        result.Should().BeSuccessful();
        (await _storeItems.GetAsync(AGetStoreItemRequest(user, item.Id), CancellationToken.None))
            .Should().BeSuccessful().And.WithValue(item with { Data = "new-data" });
    }

    [Fact]
    public async Task HandleAsync_FailsWithNotFound_WhenTheItemBelongsToAnotherUser()
    {
        var owner = AUser();
        var item = AStoreItem();
        _storeItems.Add(owner, item);

        var result = await _useCase.HandleAsync(AModifyStoreItemRequest(AUser(), item.Id), CancellationToken.None);

        result.Should().BeFailure().And.WithError(StoreItemErrors.NotFound);
        (await _storeItems.GetAsync(AGetStoreItemRequest(owner, item.Id), CancellationToken.None))
            .Should().BeSuccessful().And.WithValue(item);
    }

    [Fact]
    public async Task HandleAsync_FailsWithNotFound_WhenTheItemDoesNotExist()
    {
        var user = AUser();
        _storeItems.Add(user, AStoreItem());

        var result = await _useCase.HandleAsync(AModifyStoreItemRequest(user, AnId()), CancellationToken.None);

        result.Should().BeFailure().And.WithError(StoreItemErrors.NotFound);
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheFailure_WhenStoreItemsCannotBeWritten()
    {
        var failure = new ResultError("store-items-unavailable", "Store items are unavailable.");
        _storeItems.FailWith(failure);

        var result = await _useCase.HandleAsync(AModifyStoreItemRequest(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(failure);
    }
}
