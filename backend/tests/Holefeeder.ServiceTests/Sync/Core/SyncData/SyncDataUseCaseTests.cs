using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Sync.Core;
using Holefeeder.Service.Sync.Core.SyncData;
using Holefeeder.ServiceTests.Sync.TestKit;

using static Holefeeder.ServiceTests.Sync.TestKit.SyncRecordForTest;

namespace Holefeeder.ServiceTests.Sync.Core.SyncData;

public class SyncDataUseCaseTests
{
    private readonly SyncInMemory _store = new();
    private readonly SyncDataUseCase _useCase;
    private readonly User _user = AUser();

    public SyncDataUseCaseTests() => _useCase = new SyncDataUseCase(_store);

    [Fact]
    public async Task HandleAsync_FailsWithNotRegistered_WhenTheUserIsNotRegistered()
    {
        var result = await _useCase.HandleAsync(
            new SyncDataRequest(User.NoUser, [Put(AnId(), new SyncStoreItemPatch("code"))]), CancellationToken.None);

        result.Should().BeFailure().And.WithError(UserErrors.NotRegistered);
    }

    [Fact]
    public async Task HandleAsync_DoesNothing_WhenThereAreNoOperations()
    {
        _store.FailWith(new ResultError("store-unavailable", "Store is unavailable."));

        var result = await HandleAsync();

        result.Should().BeSuccessful();
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheFailure_WhenTheSessionCannotStart()
    {
        var failure = new ResultError("store-unavailable", "Store is unavailable.");
        _store.FailWith(failure);

        var result = await HandleAsync(Delete(AnId(), new SyncAccountPatch()));

        result.Should().BeFailure().And.WithError(failure);
    }

    [Fact]
    public async Task HandleAsync_CreatesAnAccountWithDefaults_WhenPuttingANewAccount()
    {
        var id = AnId();

        var result = await HandleAsync(Put(id, new SyncAccountPatch(Name: "Chequing", OpenDate: new DateOnly(2024, 2, 29))));

        result.Should().BeSuccessful();
        _store.Find(_user, SyncTable.Accounts, id).Should().Be(
            new SyncAccount(id, AccountType.Checking, "Chequing", Money.Zero, new DateOnly(2024, 2, 29), string.Empty, false, false));
    }

    [Fact]
    public async Task HandleAsync_FailsWithValidationErrors_WhenPuttingAnAccountWithoutNameOrOpenDate()
    {
        var result = await HandleAsync(Put(AnId(), new SyncAccountPatch(Description: "nameless")));

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>()
            .Which.Errors.Keys.Should().BeEquivalentTo(
                [SyncRecordErrors.CodeAccountNameInvalid, SyncRecordErrors.CodeAccountOpenDateRequired]);
        _store.Count.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_ChangesOnlyThePatchedFields_WhenPatchingAnAccount()
    {
        var account = ASyncAccount();
        _store.Add(_user, account);

        var result = await HandleAsync(Patch(account.Id, new SyncAccountPatch(Name: "Updated via sync", Favorite: true)));

        result.Should().BeSuccessful();
        _store.Find(_user, SyncTable.Accounts, account.Id).Should().Be(account with { Name = "Updated via sync", Favorite = true });
    }

    [Fact]
    public async Task HandleAsync_ChangesOnlyTheGivenFields_WhenPuttingAnExistingAccount()
    {
        var account = ASyncAccount();
        _store.Add(_user, account);

        var result = await HandleAsync(Put(account.Id, new SyncAccountPatch(Description: "Replaced")));

        result.Should().BeSuccessful();
        _store.Find(_user, SyncTable.Accounts, account.Id).Should().Be(account with { Description = "Replaced" });
    }

    [Fact]
    public async Task HandleAsync_FailsWithNotFound_WhenPatchingAMissingAccount()
    {
        var result = await HandleAsync(Patch(AnId(), new SyncAccountPatch(Name: "Ghost")));

        result.Should().BeFailure().And.WithError(SyncRecordErrors.AccountNotFound);
    }

    [Fact]
    public async Task HandleAsync_FailsWithNotFound_WhenPatchingAnotherUsersAccount()
    {
        var account = ASyncAccount();
        _store.Add(AUser(), account);

        var result = await HandleAsync(Patch(account.Id, new SyncAccountPatch(Name: "Stolen")));

        result.Should().BeFailure().And.WithError(SyncRecordErrors.AccountNotFound);
    }

    [Fact]
    public async Task HandleAsync_FailsWithNotFound_WhenPuttingOverAnotherUsersAccount()
    {
        var otherUser = AUser();
        var account = ASyncAccount();
        _store.Add(otherUser, account);

        var result = await HandleAsync(Put(account.Id, new SyncAccountPatch(Name: "Stolen", OpenDate: new DateOnly(2024, 1, 1))));

        result.Should().BeFailure().And.WithError(SyncRecordErrors.AccountNotFound);
        _store.Find(otherUser, SyncTable.Accounts, account.Id).Should().Be(account);
    }

    [Fact]
    public async Task HandleAsync_RemovesTheAccount_WhenDeletingAnAccount()
    {
        var account = ASyncAccount();
        _store.Add(_user, account);

        var result = await HandleAsync(Delete(account.Id, new SyncAccountPatch()));

        result.Should().BeSuccessful();
        _store.Find(_user, SyncTable.Accounts, account.Id).Should().BeNull();
    }

    [Fact]
    public async Task HandleAsync_FailsWithNotFound_WhenDeletingAMissingAccount()
    {
        var result = await HandleAsync(Delete(AnId(), new SyncAccountPatch()));

        result.Should().BeFailure().And.WithError(SyncRecordErrors.AccountNotFound);
    }

    [Fact]
    public async Task HandleAsync_AppliesNothing_WhenALaterOperationFails()
    {
        var account = ASyncAccount();
        _store.Add(_user, account);

        var result = await HandleAsync(
            Delete(account.Id, new SyncAccountPatch()),
            Patch(AnId(), new SyncAccountPatch(Name: "Ghost")));

        result.Should().BeFailure().And.WithError(SyncRecordErrors.AccountNotFound);
        _store.Find(_user, SyncTable.Accounts, account.Id).Should().Be(account);
    }

    [Fact]
    public async Task HandleAsync_SeesEarlierOperations_WhenALaterOperationPatchesWhatWasJustPut()
    {
        var id = AnId();

        var result = await HandleAsync(
            Put(id, new SyncStoreItemPatch("reminders", "first")),
            Patch(id, new SyncStoreItemPatch(Data: "second")));

        result.Should().BeSuccessful();
        _store.Find(_user, SyncTable.StoreItems, id).Should().Be(new SyncStoreItem(id, "reminders", "second"));
    }

    [Fact]
    public async Task HandleAsync_CreatesACategoryWithDefaults_WhenPuttingANewCategory()
    {
        var id = AnId();

        var result = await HandleAsync(Put(id, new SyncCategoryPatch(Name: "Groceries")));

        result.Should().BeSuccessful();
        _store.Find(_user, SyncTable.Categories, id).Should().Be(
            new SyncCategory(id, CategoryType.Expense, "Groceries", string.Empty, Money.Zero, false, false, false));
    }

    [Fact]
    public async Task HandleAsync_FailsWithValidationErrors_WhenPatchingACategoryNameTooLong()
    {
        var category = ASyncCategory();
        _store.Add(_user, category);

        var result = await HandleAsync(Patch(category.Id, new SyncCategoryPatch(Name: new string('x', 256))));

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>()
            .Which.Errors.Keys.Should().BeEquivalentTo([SyncRecordErrors.CodeCategoryNameInvalid]);
    }

    [Fact]
    public async Task HandleAsync_MakesTheCategoryInactive_WhenDeletingACategory()
    {
        var category = ASyncCategory();
        _store.Add(_user, category);

        var result = await HandleAsync(Delete(category.Id, new SyncCategoryPatch()));

        result.Should().BeSuccessful();
        _store.Find(_user, SyncTable.Categories, category.Id).Should().Be(category with { Inactive = true });
    }

    [Fact]
    public async Task HandleAsync_FailsWithValidationErrors_WhenPuttingAStoreItemWithoutCode()
    {
        var result = await HandleAsync(Put(AnId(), new SyncStoreItemPatch(Data: "orphan")));

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>()
            .Which.Errors.Keys.Should().BeEquivalentTo([SyncRecordErrors.CodeStoreItemCodeRequired]);
    }

    [Fact]
    public async Task HandleAsync_RemovesTheStoreItem_WhenDeletingAStoreItem()
    {
        var item = ASyncStoreItem();
        _store.Add(_user, item);

        var result = await HandleAsync(Delete(item.Id, new SyncStoreItemPatch()));

        result.Should().BeSuccessful();
        _store.Find(_user, SyncTable.StoreItems, item.Id).Should().BeNull();
    }

    [Fact]
    public async Task HandleAsync_CreatesATransaction_WhenPuttingANewTransaction()
    {
        var (id, accountId, categoryId) = (AnId(), AnId(), AnId());
        var tags = TagList.Create(["lunch"]);

        var result = await HandleAsync(Put(id, new SyncTransactionPatch(
            new DateOnly(2024, 5, 17), Money.Valid(42.99m), "Lunch", accountId, categoryId, Tags: tags)));

        result.Should().BeSuccessful();
        _store.Find(_user, SyncTable.Transactions, id).Should().Be(
            new SyncTransaction(id, new DateOnly(2024, 5, 17), Money.Valid(42.99m), "Lunch", accountId, categoryId, null, null, tags));
    }

    [Fact]
    public async Task HandleAsync_FailsWithValidationErrors_WhenPuttingATransactionWithoutDateAccountOrCategory()
    {
        var result = await HandleAsync(Put(AnId(), new SyncTransactionPatch(Amount: Money.Valid(1m))));

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>()
            .Which.Errors.Keys.Should().BeEquivalentTo(
                [
                    SyncRecordErrors.CodeTransactionDateRequired,
                    SyncRecordErrors.CodeTransactionAccountRequired,
                    SyncRecordErrors.CodeTransactionCategoryRequired
                ]);
    }

    [Fact]
    public async Task HandleAsync_KeepsTheCashflowLink_WhenPatchingATransactionWithoutIt()
    {
        var cashflowId = AnId();
        var transaction = ASyncTransaction(AnId(), AnId()) with { CashflowId = cashflowId, CashflowDate = new DateOnly(2024, 3, 1) };
        _store.Add(_user, transaction);

        var result = await HandleAsync(Patch(transaction.Id, new SyncTransactionPatch(Description: "Paid")));

        result.Should().BeSuccessful();
        _store.Find(_user, SyncTable.Transactions, transaction.Id).Should().Be(transaction with { Description = "Paid" });
    }

    [Fact]
    public async Task HandleAsync_ClearsTheTags_WhenPatchingATransactionWithEmptyTags()
    {
        var transaction = ASyncTransaction(AnId(), AnId());
        _store.Add(_user, transaction);

        var result = await HandleAsync(Patch(transaction.Id, new SyncTransactionPatch(Tags: TagList.Create([]))));

        result.Should().BeSuccessful();
        ((SyncTransaction)_store.Find(_user, SyncTable.Transactions, transaction.Id)!).Tags.Text.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_RemovesTheTransaction_WhenDeletingATransaction()
    {
        var transaction = ASyncTransaction(AnId(), AnId());
        _store.Add(_user, transaction);

        var result = await HandleAsync(Delete(transaction.Id, new SyncTransactionPatch()));

        result.Should().BeSuccessful();
        _store.Find(_user, SyncTable.Transactions, transaction.Id).Should().BeNull();
    }

    [Fact]
    public async Task HandleAsync_CreatesACashflowWithDefaults_WhenPuttingANewCashflow()
    {
        var (id, accountId, categoryId) = (AnId(), AnId(), AnId());

        var result = await HandleAsync(Put(id, new SyncCashflowPatch(
            new DateOnly(2024, 1, 15), Amount: Money.Valid(1500m), AccountId: accountId, CategoryId: categoryId)));

        result.Should().BeSuccessful();
        _store.Find(_user, SyncTable.Cashflows, id).Should().BeEquivalentTo(
            new SyncCashflow(id, new DateOnly(2024, 1, 15), IntervalType.Monthly, 1, 0, Money.Valid(1500m), string.Empty, accountId,
                categoryId, false, TagList.Create([])));
    }

    [Fact]
    public async Task HandleAsync_FailsWithValidationErrors_WhenPatchingACashflowWithInvalidFrequencyAndRecurrence()
    {
        var cashflow = ASyncCashflow(AnId(), AnId());
        _store.Add(_user, cashflow);

        var result = await HandleAsync(Patch(cashflow.Id, new SyncCashflowPatch(Frequency: 0, Recurrence: -1)));

        result.Should().BeFailure();
        result.Error.Should().BeOfType<ResultErrorAggregate>()
            .Which.Errors.Keys.Should().BeEquivalentTo(
                [SyncRecordErrors.CodeCashflowFrequencyInvalid, SyncRecordErrors.CodeCashflowRecurrenceInvalid]);
    }

    [Fact]
    public async Task HandleAsync_ChangesOnlyThePatchedFields_WhenPatchingACashflow()
    {
        var cashflow = ASyncCashflow(AnId(), AnId());
        _store.Add(_user, cashflow);

        var result = await HandleAsync(Patch(cashflow.Id, new SyncCashflowPatch(Amount: Money.Valid(99m), Inactive: true)));

        result.Should().BeSuccessful();
        _store.Find(_user, SyncTable.Cashflows, cashflow.Id).Should().Be(cashflow with { Amount = Money.Valid(99m), Inactive = true });
    }

    [Fact]
    public async Task HandleAsync_RemovesTheCashflow_WhenDeletingACashflow()
    {
        var cashflow = ASyncCashflow(AnId(), AnId());
        _store.Add(_user, cashflow);

        var result = await HandleAsync(Delete(cashflow.Id, new SyncCashflowPatch()));

        result.Should().BeSuccessful();
        _store.Find(_user, SyncTable.Cashflows, cashflow.Id).Should().BeNull();
    }

    private Task<Result<Nothing>> HandleAsync(params SyncOperation[] operations) =>
        _useCase.HandleAsync(new SyncDataRequest(_user, operations), CancellationToken.None);

    private static SyncOperation Put(Id id, ISyncPatch patch) => new(SyncOperationKind.Put, id, patch);

    private static SyncOperation Patch(Id id, ISyncPatch patch) => new(SyncOperationKind.Patch, id, patch);

    private static SyncOperation Delete(Id id, ISyncPatch patch) => new(SyncOperationKind.Delete, id, patch);
}
