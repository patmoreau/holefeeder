using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.StoreItems.Core;
using Holefeeder.Service.StoreItems.Core.CreateStoreItem;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.StoreItems.TestKit;

namespace Holefeeder.ServiceTests.StoreItems.Core.CreateStoreItem;

public class CreateStoreItemUseCaseTests
{
    private readonly StoreItemsInMemory _storeItems = new();
    private readonly CreateStoreItemUseCase _useCase;

    public CreateStoreItemUseCaseTests() => _useCase = new CreateStoreItemUseCase(_storeItems);

    [Fact]
    public async Task HandleAsync_CreatesTheItemForTheUser_WhenTheCodeIsNew()
    {
        var user = AUser();
        var code = ACode();
        _storeItems.Add(user, AStoreItem());

        var result = await _useCase.HandleAsync(
            ACreateStoreItemRequest(user, code, Data.Valid("new-data")), CancellationToken.None);

        result.Should().BeSuccessful();
        (await _storeItems.GetAsync(AGetStoreItemRequest(user, result.Value), CancellationToken.None))
            .Should().BeSuccessful().And.WithValue(StoreItem.Valid(result.Value, code, "new-data"));
    }

    [Fact]
    public async Task HandleAsync_CreatesTheItem_WhenAnotherUserHasTheCode()
    {
        var code = ACode();
        _storeItems.Add(AUser(), AStoreItem(code));

        var result = await _useCase.HandleAsync(ACreateStoreItemRequest(AUser(), code), CancellationToken.None);

        result.Should().BeSuccessful();
    }

    [Fact]
    public async Task HandleAsync_FailsWithCodeAlreadyExists_WhenTheUserHasTheCode()
    {
        var user = AUser();
        var code = ACode();
        _storeItems.Add(user, AStoreItem(code));

        var result = await _useCase.HandleAsync(ACreateStoreItemRequest(user, code), CancellationToken.None);

        result.Should().BeFailure().And.WithError(StoreItemErrors.CodeAlreadyExists);
    }

    [Fact]
    public async Task HandleAsync_FailsWithUserNotRegistered_WhenThereIsNoUser()
    {
        var result = await _useCase.HandleAsync(ACreateStoreItemRequest(User.NoUser), CancellationToken.None);

        result.Should().BeFailure().And.WithError(UserErrors.NotRegistered);
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheFailure_WhenStoreItemsCannotBeWritten()
    {
        var failure = new ResultError("store-items-unavailable", "Store items are unavailable.");
        _storeItems.FailWith(failure);

        var result = await _useCase.HandleAsync(ACreateStoreItemRequest(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(failure);
    }

    [Fact]
    public void CodeAlreadyExists_DescribesTheUniqueCodeRule()
    {
        StoreItemErrors.CodeAlreadyExists.Code.Should().Be("store-item-code-already-exists");
        StoreItemErrors.CodeAlreadyExists.Description.Should().Be("Store item code already exists.");
    }

    [Fact]
    public void UserNotRegistered_DescribesTheRegistrationRule()
    {
        UserErrors.NotRegistered.Code.Should().Be("user-not-registered");
        UserErrors.NotRegistered.Description.Should().Be("User is not registered.");
    }
}
