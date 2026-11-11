using Holefeeder.Service.Dashboard.Core.GetDashboardAccounts;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Transactions.Core;
using Holefeeder.Service.Transactions.Persistence;
using Holefeeder.ServiceTests.Dashboard.TestKit;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Persistence;
using Holefeeder.ServiceTests.Transactions.TestKit;

namespace Holefeeder.ServiceTests.Transactions.Persistence;

[Collection(DatabaseTestMarker.Name)]
public class TransactionsInDatabaseTests(DatabaseForTest database, ITestOutputHelper testOutputHelper)
{
    private readonly TransactionsInDatabase _repository = new(database.DataSource, new LoggerForTest<TransactionsInDatabase>(testOutputHelper));

    [Fact]
    public async Task GetOpenAccountsAsync_ListsTheUsersOpenAccounts()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        var chequing = await budget.AnAccountAsync("Chequing", AccountType.Checking, favorite: true);
        var amex = await budget.AnAccountAsync("Amex", AccountType.CreditCard);
        await budget.AnAccountAsync("Closed", AccountType.Checking, inactive: true);
        var otherUser = await AUser().SavedInDbAsync(database);
        await new BudgetForTest(otherUser, database).AnAccountAsync("Other", AccountType.Checking);

        var result = await _repository.GetOpenAccountsAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo([
            new TransactionAccount(Id.Valid(chequing), "Chequing", true),
            new TransactionAccount(Id.Valid(amex), "Amex", false)
        ]);
    }

    [Fact]
    public async Task GetActiveCategoriesAsync_ListsTheUsersCategoriesExceptSystemAndInactiveOnes()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        var groceries = await budget.ACategoryAsync(CategoryType.Expense, name: "Groceries", color: "#34C759", favorite: true);
        var salary = await budget.ACategoryAsync(CategoryType.Gain, name: "Salary", color: "#007AFF");
        await budget.ACategoryAsync(CategoryType.Expense, system: true, name: "Transfer Out");
        await budget.ACategoryAsync(CategoryType.Expense, name: "Retired", inactive: true);
        var otherUser = await AUser().SavedInDbAsync(database);
        await new BudgetForTest(otherUser, database).ACategoryAsync(CategoryType.Expense, name: "Other");

        var result = await _repository.GetActiveCategoriesAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo([
            new TransactionCategory(Id.Valid(groceries), "Groceries", "#34C759", CategoryType.Expense, true),
            new TransactionCategory(Id.Valid(salary), "Salary", "#007AFF", CategoryType.Gain, false)
        ]);
    }

    [Fact]
    public async Task GetOpenAccountsAsync_Fails_WhenTheDatabaseIsUnreachable()
    {
        var repository = new TransactionsInDatabase(database.BadDataSource, new LoggerForTest<TransactionsInDatabase>(testOutputHelper));

        var result = await repository.GetOpenAccountsAsync(AUser(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(TransactionsErrors.ReadFailed);
    }

    [Fact]
    public async Task GetActiveCategoriesAsync_Fails_WhenTheDatabaseIsUnreachable()
    {
        var repository = new TransactionsInDatabase(database.BadDataSource, new LoggerForTest<TransactionsInDatabase>(testOutputHelper));

        var result = await repository.GetActiveCategoriesAsync(AUser(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(TransactionsErrors.ReadFailed);
    }

    [Fact]
    public async Task GetTagUsagesAsync_CountsEachTagPerCategory()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        var groceries = await budget.ACategoryAsync(CategoryType.Expense);
        var dining = await budget.ACategoryAsync(CategoryType.Expense);
        var day = new DateOnly(2026, 11, 1);
        await budget.ATransactionAsync(groceries, day, 10m, tags: "weekly,costco");
        await budget.ATransactionAsync(groceries, day, 10m, tags: "Weekly, ,family");
        await budget.ATransactionAsync(dining, day, 10m, tags: "family");
        await budget.ATransactionAsync(dining, day, 10m, tags: null);
        var otherUser = await AUser().SavedInDbAsync(database);
        var otherBudget = new BudgetForTest(otherUser, database);
        await otherBudget.ATransactionAsync(await otherBudget.ACategoryAsync(CategoryType.Expense), day, 10m, tags: "weekly");

        var result = await _repository.GetTagUsagesAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo([
            new TagUsage("weekly", Id.Valid(groceries), 2),
            new TagUsage("costco", Id.Valid(groceries), 1),
            new TagUsage("family", Id.Valid(groceries), 1),
            new TagUsage("family", Id.Valid(dining), 1)
        ]);
    }

    [Fact]
    public async Task GetTagUsagesAsync_Fails_WhenTheDatabaseIsUnreachable()
    {
        var repository = new TransactionsInDatabase(database.BadDataSource, new LoggerForTest<TransactionsInDatabase>(testOutputHelper));

        var result = await repository.GetTagUsagesAsync(AUser(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(TransactionsErrors.ReadFailed);
    }

    [Fact]
    public async Task GetOpenAccountAsync_ReturnsTheUsersOpenAccount()
    {
        var user = await AUser().SavedInDbAsync(database);
        var chequing = await new BudgetForTest(user, database).AnAccountAsync("Chequing", AccountType.Checking, favorite: true);

        var result = await _repository.GetOpenAccountAsync(user, Id.Valid(chequing), CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(new TransactionAccount(Id.Valid(chequing), "Chequing", true));
    }

    [Fact]
    public async Task GetOpenAccountAsync_Fails_WhenTheAccountIsClosedOrSomeoneElses()
    {
        var user = await AUser().SavedInDbAsync(database);
        var closed = await new BudgetForTest(user, database).AnAccountAsync("Closed", AccountType.Checking, inactive: true);
        var otherUser = await AUser().SavedInDbAsync(database);
        var others = await new BudgetForTest(otherUser, database).AnAccountAsync("Other", AccountType.Checking);

        var closedResult = await _repository.GetOpenAccountAsync(user, Id.Valid(closed), CancellationToken.None);
        var othersResult = await _repository.GetOpenAccountAsync(user, Id.Valid(others), CancellationToken.None);

        closedResult.Should().BeFailure().And.WithError(TransactionErrors.AccountNotFound);
        othersResult.Should().BeFailure().And.WithError(TransactionErrors.AccountNotFound);
    }

    [Fact]
    public async Task GetActiveCategoryAsync_ReturnsTheUsersCategory()
    {
        var user = await AUser().SavedInDbAsync(database);
        var salary = await new BudgetForTest(user, database).ACategoryAsync(CategoryType.Gain, name: "Salary", color: "#007AFF");

        var result = await _repository.GetActiveCategoryAsync(user, Id.Valid(salary), CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(new TransactionCategory(Id.Valid(salary), "Salary", "#007AFF", CategoryType.Gain, false));
    }

    [Fact]
    public async Task GetActiveCategoryAsync_Fails_WhenTheCategoryIsSystemInactiveOrSomeoneElses()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        var system = await budget.ACategoryAsync(CategoryType.Expense, system: true);
        var inactive = await budget.ACategoryAsync(CategoryType.Expense, inactive: true);
        var otherUser = await AUser().SavedInDbAsync(database);
        var others = await new BudgetForTest(otherUser, database).ACategoryAsync(CategoryType.Expense);

        foreach (var id in new[] { system, inactive, others })
        {
            var result = await _repository.GetActiveCategoryAsync(user, Id.Valid(id), CancellationToken.None);

            result.Should().BeFailure().And.WithError(TransactionErrors.CategoryNotFound);
        }
    }

    [Fact]
    public async Task SaveAsync_RecordsTheTransactionsAndTheCashflow()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        var account = Id.Valid(await budget.AnAccountAsync("Chequing", AccountType.Checking));
        var category = Id.Valid(await budget.ACategoryAsync(CategoryType.Expense));
        var tags = Tags.Create(["weekly", "costco"]);
        var cashflow = new Cashflow(AnId(), new DateOnly(2026, 11, 15), Money.Valid(42.50m), IntervalType.Monthly, Frequency.Valid(2),
            "Market", account, category, tags);
        var purchase = new Transaction(AnId(), new DateOnly(2026, 11, 11), Money.Valid(42.50m), "Market", account, category, tags,
            cashflow.Id, new DateOnly(2026, 11, 11));
        var other = new Transaction(AnId(), new DateOnly(2026, 11, 11), Money.Valid(500m), "", account, category, Tags.Create([]),
            null, null);

        var result = await _repository.SaveAsync(user, [purchase, other], cashflow, CancellationToken.None);

        result.Should().BeSuccessful();
        (await database.CashflowsInDbAsync(account.Value)).Should().Equal(new CashflowInDb(
            cashflow.Id.Value, new DateOnly(2026, 11, 15), 42.50m, "Monthly", 2, 0, "Market", account.Value, category.Value, false,
            "weekly,costco", user.Id.Value));
        (await database.TransactionsInDbAsync(account.Value)).Should().Equal(
            new TransactionInDb(purchase.Id.Value, new DateOnly(2026, 11, 11), 42.50m, "Market", account.Value, category.Value,
                cashflow.Id.Value, new DateOnly(2026, 11, 11), "weekly,costco", user.Id.Value),
            new TransactionInDb(other.Id.Value, new DateOnly(2026, 11, 11), 500m, "", account.Value, category.Value, null, null, "",
                user.Id.Value));
    }

    [Fact]
    public async Task SaveAsync_SavesNothing_WhenAnyPartFails()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        var account = Id.Valid(await budget.AnAccountAsync("Chequing", AccountType.Checking));
        var category = Id.Valid(await budget.ACategoryAsync(CategoryType.Expense));
        var cashflow = new Cashflow(AnId(), new DateOnly(2026, 11, 15), Money.Valid(42.50m), IntervalType.Monthly, Frequency.Valid(1),
            "", account, category, Tags.Create([]));
        var valid = new Transaction(AnId(), new DateOnly(2026, 11, 11), Money.Valid(42.50m), "", account, category, Tags.Create([]),
            cashflow.Id, new DateOnly(2026, 11, 11));
        var unknownCategory = valid with { Id = AnId(), CategoryId = AnId(), CashflowId = null, CashflowDate = null };

        var result = await _repository.SaveAsync(user, [valid, unknownCategory], cashflow, CancellationToken.None);

        result.Should().BeFailure().And.WithError(TransactionsErrors.WriteFailed);
        (await database.CashflowsInDbAsync(account.Value)).Should().BeEmpty();
        (await database.TransactionsInDbAsync(account.Value)).Should().BeEmpty();
    }

    [Fact]
    public async Task GetTransferCategoriesAsync_FindsTheUsersTransferCategories()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        var transferOut = await budget.ACategoryAsync(CategoryType.Expense, system: true, name: "Transfer Out");
        var transferIn = await budget.ACategoryAsync(CategoryType.Gain, system: true, name: "Transfer In");
        var otherUser = await AUser().SavedInDbAsync(database);
        await new BudgetForTest(otherUser, database).ACategoryAsync(CategoryType.Expense, system: true, name: "Transfer Out");

        var result = await _repository.GetTransferCategoriesAsync(user, CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(new TransferCategories(Id.Valid(transferOut), Id.Valid(transferIn)));
    }

    [Fact]
    public async Task GetTransferCategoriesAsync_Fails_WhenOneIsMissing()
    {
        var user = await AUser().SavedInDbAsync(database);
        await new BudgetForTest(user, database).ACategoryAsync(CategoryType.Expense, system: true, name: "Transfer Out");

        var result = await _repository.GetTransferCategoriesAsync(user, CancellationToken.None);

        result.Should().BeFailure().And.WithError(TransactionErrors.TransferCategoriesNotFound);
    }
}
