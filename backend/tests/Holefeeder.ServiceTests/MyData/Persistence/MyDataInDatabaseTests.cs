using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.MyData.Core;
using Holefeeder.Service.MyData.Persistence;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Persistence;

using static Holefeeder.ServiceTests.MyData.TestKit.MyDataForTest;

namespace Holefeeder.ServiceTests.MyData.Persistence;

[Collection(DatabaseTestMarker.Name)]
public class MyDataInDatabaseTests(DatabaseForTest database, ITestOutputHelper testOutputHelper)
{
    private readonly MyDataInDatabase _store = new(database.DataSource, new LoggerForTest<MyDataInDatabase>(testOutputHelper));

    [Fact]
    public async Task ExportAsync_ReadsEveryTableOfTheUser()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        var accountId = await budget.AnAccountAsync("Visa", AccountType.CreditCard, openBalance: 12.5m, openDate: new DateOnly(2021, 2, 3),
            favorite: true, description: "Travel");
        var categoryId = await budget.ACategoryAsync(CategoryType.Gain, name: "Salary", color: "#00FF00", favorite: true, budget: 2500m);
        var cashflowId = await budget.ACashflowAsync(categoryId, new DateOnly(2024, 1, 15), IntervalType.Weekly, 2, 1500m, inactive: true,
            accountId: accountId, description: "Pay", tags: "work,weekly");
        var transactionId = await budget.ATransactionAsync(categoryId, new DateOnly(2024, 5, 17), 42.99m, accountId, "Lunch", "food");
        await budget.APaymentAsync(cashflowId, categoryId, new DateOnly(2024, 1, 16), new DateOnly(2024, 1, 15), 1500m);

        var result = await _store.ExportAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        var data = result.Value;
        data.Accounts.Should().Contain(new MyDataAccount(Id.Valid(accountId), AccountType.CreditCard, "Visa", Money.Valid(12.5m),
            new DateOnly(2021, 2, 3), "Travel", Favorite: true, Inactive: false));
        data.Categories.Should().Equal(new MyDataCategory(Id.Valid(categoryId), CategoryType.Gain, "Salary", "#00FF00",
            Money.Valid(2500m), Favorite: true, System: false, Inactive: false));
        data.Cashflows.Should().Equal(new MyDataCashflow(Id.Valid(cashflowId), new DateOnly(2024, 1, 15), Money.Valid(1500m),
            IntervalType.Weekly, 2, 0, "Pay", TagList.Create(["work", "weekly"]), Id.Valid(categoryId), Id.Valid(accountId),
            Inactive: true));
        data.Transactions.Should().HaveCount(2).And.Contain(new MyDataTransaction(Id.Valid(transactionId), new DateOnly(2024, 5, 17),
            Money.Valid(42.99m), "Lunch", TagList.Create(["food"]), Id.Valid(categoryId), Id.Valid(accountId), null, null));
        data.Transactions.Should().ContainSingle(transaction => transaction.CashflowId == Id.Valid(cashflowId))
            .Which.CashflowDate.Should().Be(new DateOnly(2024, 1, 15));
    }

    [Fact]
    public async Task ExportAsync_LeavesOutOtherUsersData()
    {
        var user = await AUser().SavedInDbAsync(database);
        var otherBudget = new BudgetForTest(await AUser().SavedInDbAsync(database), database);
        var categoryId = await otherBudget.ACategoryAsync(CategoryType.Expense);
        await otherBudget.ATransactionAsync(categoryId, new DateOnly(2024, 1, 1), 1m);

        var result = await _store.ExportAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo(new MyDataSet([], [], [], []));
    }

    [Fact]
    public async Task ExportAsync_FailsWithReadFailed_WhenTheDatabaseIsUnreachable()
    {
        var store = new MyDataInDatabase(database.BadDataSource, new LoggerForTest<MyDataInDatabase>(testOutputHelper));

        var result = await store.ExportAsync(AUser(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(MyDataDatabaseErrors.ReadFailed);
    }

    [Fact]
    public async Task Import_SavesEveryTable_ThatExportReadsBack()
    {
        var user = await AUser().SavedInDbAsync(database);
        var (account, category) = (AMyDataAccount(), AMyDataCategory());
        var cashflow = AMyDataCashflow(account.Id, category.Id);
        var transaction = AMyDataTransaction(account.Id, category.Id) with
        {
            CashflowId = cashflow.Id,
            CashflowDate = new DateOnly(2024, 1, 15)
        };

        await using (var import = (await _store.BeginImportAsync(user, CancellationToken.None)).Value)
        {
            (await import.SaveAsync(account, CancellationToken.None)).Should().BeSuccessful();
            (await import.SaveAsync(category, CancellationToken.None)).Should().BeSuccessful();
            (await import.SaveAsync(cashflow, CancellationToken.None)).Should().BeSuccessful();
            (await import.SaveAsync(transaction, CancellationToken.None)).Should().BeSuccessful();
            (await import.CommitAsync(CancellationToken.None)).Should().BeSuccessful();
        }

        (await _store.ExportAsync(user, CancellationToken.None)).Value.Should()
            .BeEquivalentTo(new MyDataSet([account], [category], [cashflow], [transaction]));
    }

    [Fact]
    public async Task Import_ReplacesAnExistingRecord()
    {
        var user = await AUser().SavedInDbAsync(database);
        var account = AMyDataAccount();
        await ImportAsync(user, account);
        var changed = account with { Name = "Renamed", OpenBalance = Money.Valid(1m), Favorite = true, Inactive = true };

        await ImportAsync(user, changed);

        (await _store.ExportAsync(user, CancellationToken.None)).Value.Accounts.Should().Equal(changed);
    }

    [Fact]
    public async Task ExistsAsync_TellsWhetherTheUserOwnsTheRecord()
    {
        var user = await AUser().SavedInDbAsync(database);
        var other = await AUser().SavedInDbAsync(database);
        var account = AMyDataAccount();
        await ImportAsync(other, account);

        await using var import = (await _store.BeginImportAsync(user, CancellationToken.None)).Value;

        (await import.ExistsAsync(MyDataTable.Accounts, account.Id, CancellationToken.None)).Should().BeSuccessful().And.WithValue(false);
        (await import.SaveAsync(AMyDataAccount(account.Id), CancellationToken.None)).Should().BeFailure()
            .And.WithError(MyDataImportErrors.IdTaken(MyDataTable.Accounts));
    }

    [Fact]
    public async Task ExistsAsync_FindsTheUsersRecord()
    {
        var user = await AUser().SavedInDbAsync(database);
        var category = AMyDataCategory();
        await ImportAsync(user, category);

        await using var import = (await _store.BeginImportAsync(user, CancellationToken.None)).Value;

        (await import.ExistsAsync(MyDataTable.Categories, category.Id, CancellationToken.None)).Should().BeSuccessful()
            .And.WithValue(true);
        (await import.ExistsAsync(MyDataTable.Cashflows, category.Id, CancellationToken.None)).Should().BeSuccessful()
            .And.WithValue(false);
    }

    [Fact]
    public async Task DisposeAsync_DiscardsTheImport_WhenNotCommitted()
    {
        var user = await AUser().SavedInDbAsync(database);

        await using (var import = (await _store.BeginImportAsync(user, CancellationToken.None)).Value)
        {
            (await import.SaveAsync(AMyDataAccount(), CancellationToken.None)).Should().BeSuccessful();
        }

        (await _store.ExportAsync(user, CancellationToken.None)).Value.Accounts.Should().BeEmpty();
    }

    [Fact]
    public async Task SaveAsync_FailsWithWriteFailed_WhenAReferenceIsUnknown()
    {
        var user = await AUser().SavedInDbAsync(database);

        await using var import = (await _store.BeginImportAsync(user, CancellationToken.None)).Value;
        var result = await import.SaveAsync(AMyDataTransaction(AnId(), AnId()), CancellationToken.None);

        result.Should().BeFailure().And.WithError(MyDataDatabaseErrors.WriteFailed);
    }

    [Fact]
    public async Task BeginImportAsync_FailsWithWriteFailed_WhenTheDatabaseIsUnreachable()
    {
        var store = new MyDataInDatabase(database.BadDataSource, new LoggerForTest<MyDataInDatabase>(testOutputHelper));

        var result = await store.BeginImportAsync(AUser(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(MyDataDatabaseErrors.WriteFailed);
    }

    [Fact]
    public void DatabaseErrors_AreUnexpectedErrors()
    {
        MyDataDatabaseErrors.ReadFailed.Should().BeOfType<UnexpectedError>();
        MyDataDatabaseErrors.WriteFailed.Should().BeOfType<UnexpectedError>();
    }

    private async Task ImportAsync(Service.Support.Core.User user, MyDataAccount account)
    {
        await using var import = (await _store.BeginImportAsync(user, CancellationToken.None)).Value;
        (await import.SaveAsync(account, CancellationToken.None)).Should().BeSuccessful();
        (await import.CommitAsync(CancellationToken.None)).Should().BeSuccessful();
    }

    private async Task ImportAsync(Service.Support.Core.User user, MyDataCategory category)
    {
        await using var import = (await _store.BeginImportAsync(user, CancellationToken.None)).Value;
        (await import.SaveAsync(category, CancellationToken.None)).Should().BeSuccessful();
        (await import.CommitAsync(CancellationToken.None)).Should().BeSuccessful();
    }
}
