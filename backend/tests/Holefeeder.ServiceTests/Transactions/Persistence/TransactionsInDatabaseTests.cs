using Holefeeder.Service.Dashboard.Core.GetDashboardAccounts;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Transactions.Core;
using Holefeeder.Service.Transactions.Persistence;
using Holefeeder.ServiceTests.Dashboard.TestKit;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Persistence;

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
}
