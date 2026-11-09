using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.StoreItems.Core;
using Holefeeder.Service.StoreItems.Core.GetStoreItem;
using Holefeeder.ServiceTests.StoreItems.TestKit;

namespace Holefeeder.ServiceTests.StoreItems.Core.GetStoreItem;

public class GetStoreItemUseCaseTests
{
    private readonly StoreItemsInMemory _storeItems = new();
    private readonly GetStoreItemUseCase _useCase;

    public GetStoreItemUseCaseTests() => _useCase = new GetStoreItemUseCase(_storeItems);

    [Fact]
    public async Task HandleAsync_ReturnsTheItem_WhenTheUserOwnsIt()
    {
        var user = AUser();
        var item = AStoreItem();
        _storeItems.Add(user, item);
        _storeItems.Add(user, AStoreItem());

        var result = await _useCase.HandleAsync(AGetStoreItemRequest(user, item.Id), CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(item);
    }

    [Fact]
    public async Task HandleAsync_FailsWithNotFound_WhenTheItemBelongsToAnotherUser()
    {
        var item = AStoreItem();
        _storeItems.Add(AUser(), item);

        var result = await _useCase.HandleAsync(AGetStoreItemRequest(AUser(), item.Id), CancellationToken.None);

        result.Should().BeFailure().And.WithError(StoreItemErrors.NotFound);
    }

    [Fact]
    public async Task HandleAsync_FailsWithNotFound_WhenTheItemDoesNotExist()
    {
        var user = AUser();
        _storeItems.Add(user, AStoreItem());

        var result = await _useCase.HandleAsync(AGetStoreItemRequest(user, AnId()), CancellationToken.None);

        result.Should().BeFailure().And.WithError(StoreItemErrors.NotFound);
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheFailure_WhenStoreItemsCannotBeRead()
    {
        var failure = new ResultError("store-items-unavailable", "Store items are unavailable.");
        _storeItems.FailWith(failure);

        var result = await _useCase.HandleAsync(AGetStoreItemRequest(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(failure);
    }
}
