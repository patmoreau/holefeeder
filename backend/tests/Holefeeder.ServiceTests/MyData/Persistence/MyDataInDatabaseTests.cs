using Holefeeder.Service.MyData.Core;
using Holefeeder.Service.MyData.Persistence;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Persistence;

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
    public void DatabaseErrors_AreUnexpectedErrors() =>
        MyDataDatabaseErrors.ReadFailed.Should().BeOfType<UnexpectedError>();
}
