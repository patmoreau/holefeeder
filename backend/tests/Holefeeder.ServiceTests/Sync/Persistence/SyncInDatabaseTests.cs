using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Sync.Core;
using Holefeeder.Service.Sync.Persistence;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Persistence;

using static Holefeeder.ServiceTests.Sync.TestKit.SyncRecordForTest;

namespace Holefeeder.ServiceTests.Sync.Persistence;

[Collection(DatabaseTestMarker.Name)]
public class SyncInDatabaseTests(DatabaseForTest database, ITestOutputHelper testOutputHelper)
{
    private readonly SyncInDatabase _store = new(database.DataSource, new LoggerForTest<SyncInDatabase>(testOutputHelper));

    [Fact]
    public async Task SaveAsync_StoresAnAccount_ThatFindAsyncReadsBack()
    {
        var user = await AUser().SavedInDbAsync(database);
        var account = ASyncAccount() with { Type = AccountType.Savings, Favorite = true };

        await SaveAndCommitAsync(user, account);

        (await FindAsync(user, SyncTable.Accounts, account.Id)).Should().BeSuccessful().And.WithValue(account);
    }

    [Fact]
    public async Task SaveAsync_UpdatesAnExistingAccount()
    {
        var user = await AUser().SavedInDbAsync(database);
        var account = ASyncAccount();
        await SaveAndCommitAsync(user, account);
        var changed = account with { Name = "Renamed", OpenBalance = Money.Valid(12.34m), Inactive = true };

        await SaveAndCommitAsync(user, changed);

        (await FindAsync(user, SyncTable.Accounts, account.Id)).Should().BeSuccessful().And.WithValue(changed);
    }

    [Fact]
    public async Task SaveAsync_StoresACategory_ThatFindAsyncReadsBack()
    {
        var user = await AUser().SavedInDbAsync(database);
        var category = ASyncCategory() with { Type = CategoryType.Gain, System = true, Inactive = true };

        await SaveAndCommitAsync(user, category);

        (await FindAsync(user, SyncTable.Categories, category.Id)).Should().BeSuccessful().And.WithValue(category);
    }

    [Fact]
    public async Task SaveAsync_StoresAStoreItem_ThatFindAsyncReadsBack()
    {
        var user = await AUser().SavedInDbAsync(database);
        var item = ASyncStoreItem();

        await SaveAndCommitAsync(user, item);

        (await FindAsync(user, SyncTable.StoreItems, item.Id)).Should().BeSuccessful().And.WithValue(item);
    }

    [Fact]
    public async Task SaveAsync_StoresATransaction_ThatFindAsyncReadsBack()
    {
        var user = await AUser().SavedInDbAsync(database);
        var (account, category) = (ASyncAccount(), ASyncCategory());
        var cashflow = ASyncCashflow(account.Id, category.Id);
        var transaction = ASyncTransaction(account.Id, category.Id) with
        {
            CashflowId = cashflow.Id,
            CashflowDate = new DateOnly(2024, 3, 1)
        };

        await SaveAndCommitAsync(user, account, category, cashflow, transaction);

        (await FindAsync(user, SyncTable.Transactions, transaction.Id)).Should().BeSuccessful().And.WithValue(transaction);
    }

    [Fact]
    public async Task SaveAsync_StoresACashflow_ThatFindAsyncReadsBack()
    {
        var user = await AUser().SavedInDbAsync(database);
        var (account, category) = (ASyncAccount(), ASyncCategory());
        var cashflow = ASyncCashflow(account.Id, category.Id) with { IntervalType = IntervalType.Weekly, Frequency = 2, Inactive = true };

        await SaveAndCommitAsync(user, account, category, cashflow);

        (await FindAsync(user, SyncTable.Cashflows, cashflow.Id)).Should().BeSuccessful().And.WithValue(cashflow);
    }

    [Fact]
    public async Task SaveAsync_FailsWithNotFound_WhenTheRowBelongsToAnotherUser()
    {
        var owner = await AUser().SavedInDbAsync(database);
        var intruder = await AUser().SavedInDbAsync(database);
        var item = ASyncStoreItem();
        await SaveAndCommitAsync(owner, item);

        await using var session = (await _store.BeginAsync(intruder, CancellationToken.None)).Value;
        var result = await session.SaveAsync(item with { Data = "stolen" }, CancellationToken.None);

        result.Should().BeFailure().And.WithError(SyncRecordErrors.StoreItemNotFound);
        (await FindAsync(owner, SyncTable.StoreItems, item.Id)).Should().BeSuccessful().And.WithValue(item);
    }

    [Fact]
    public async Task FindAsync_FailsWithNotFound_WhenTheRowBelongsToAnotherUser()
    {
        var owner = await AUser().SavedInDbAsync(database);
        var item = ASyncStoreItem();
        await SaveAndCommitAsync(owner, item);

        var result = await FindAsync(await AUser().SavedInDbAsync(database), SyncTable.StoreItems, item.Id);

        result.Should().BeFailure().And.WithError(SyncRecordErrors.StoreItemNotFound);
    }

    [Fact]
    public async Task DeleteAsync_RemovesTheRow()
    {
        var user = await AUser().SavedInDbAsync(database);
        var item = ASyncStoreItem();
        await SaveAndCommitAsync(user, item);

        await using (var session = (await _store.BeginAsync(user, CancellationToken.None)).Value)
        {
            (await session.DeleteAsync(SyncTable.StoreItems, item.Id, CancellationToken.None)).Should().BeSuccessful();
            (await session.CommitAsync(CancellationToken.None)).Should().BeSuccessful();
        }

        (await FindAsync(user, SyncTable.StoreItems, item.Id)).Should().BeFailure().And.WithError(SyncRecordErrors.StoreItemNotFound);
    }

    [Fact]
    public async Task DeleteAsync_FailsWithNotFound_WhenTheRowDoesNotExist()
    {
        var user = await AUser().SavedInDbAsync(database);

        await using var session = (await _store.BeginAsync(user, CancellationToken.None)).Value;
        var result = await session.DeleteAsync(SyncTable.Cashflows, AnId(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(SyncRecordErrors.CashflowNotFound);
    }

    [Fact]
    public async Task DisposeAsync_DiscardsTheChanges_WhenNotCommitted()
    {
        var user = await AUser().SavedInDbAsync(database);
        var item = ASyncStoreItem();

        await using (var session = (await _store.BeginAsync(user, CancellationToken.None)).Value)
        {
            (await session.SaveAsync(item, CancellationToken.None)).Should().BeSuccessful();
        }

        (await FindAsync(user, SyncTable.StoreItems, item.Id)).Should().BeFailure().And.WithError(SyncRecordErrors.StoreItemNotFound);
    }

    [Fact]
    public async Task SaveAsync_FailsWithWriteFailed_WhenAConstraintIsViolated()
    {
        var user = await AUser().SavedInDbAsync(database);
        var transaction = ASyncTransaction(AnId(), AnId());

        await using var session = (await _store.BeginAsync(user, CancellationToken.None)).Value;
        var result = await session.SaveAsync(transaction, CancellationToken.None);

        result.Should().BeFailure().And.WithError(SyncDatabaseErrors.WriteFailed);
    }

    [Fact]
    public async Task BeginAsync_FailsWithWriteFailed_WhenTheDatabaseIsUnreachable()
    {
        var store = new SyncInDatabase(database.BadDataSource, new LoggerForTest<SyncInDatabase>(testOutputHelper));

        var result = await store.BeginAsync(AUser(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(SyncDatabaseErrors.WriteFailed);
    }

    [Fact]
    public void DatabaseErrors_AreUnexpectedErrors()
    {
        SyncDatabaseErrors.ReadFailed.Should().BeOfType<UnexpectedError>();
        SyncDatabaseErrors.WriteFailed.Should().BeOfType<UnexpectedError>();
    }

    [Fact]
    public void NotFoundErrors_AreNotFoundErrors() =>
        Enum.GetValues<SyncTable>().Select(SyncRecordErrors.NotFound).Should().AllBeOfType<NotFoundError>();

    private async Task SaveAndCommitAsync(User user, params ISyncRecord[] records)
    {
        await using var session = (await _store.BeginAsync(user, CancellationToken.None)).Value;
        foreach (var record in records)
        {
            (await session.SaveAsync(record, CancellationToken.None)).Should().BeSuccessful();
        }

        (await session.CommitAsync(CancellationToken.None)).Should().BeSuccessful();
    }

    private async Task<Result<ISyncRecord>> FindAsync(User user, SyncTable table, Id id)
    {
        await using var session = (await _store.BeginAsync(user, CancellationToken.None)).Value;
        return await session.FindAsync(table, id, CancellationToken.None);
    }
}
