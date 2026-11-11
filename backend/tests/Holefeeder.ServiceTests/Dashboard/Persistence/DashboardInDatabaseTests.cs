using Holefeeder.Service.Dashboard.Core;
using Holefeeder.Service.Dashboard.Core.GetDashboardAccounts;
using Holefeeder.Service.Dashboard.Core.GetDashboardSummary;
using Holefeeder.Service.Dashboard.Core.GetRecentTransactions;
using Holefeeder.Service.Dashboard.Core.GetUpcomingFlows;
using Holefeeder.Service.Dashboard.Persistence;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.Dashboard.TestKit;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.Dashboard.Persistence;

[Collection(DatabaseTestMarker.Name)]
public class DashboardInDatabaseTests(DatabaseForTest database, ITestOutputHelper testOutputHelper)
{
    private static readonly DateOnly Today = new(2026, 11, 10);

    private readonly DashboardInDatabase _repository = new(database.DataSource, new LoggerForTest<DashboardInDatabase>(testOutputHelper));

    [Fact]
    public async Task GetDailyTotalsAsync_SumsTransactionsByCategoryTypeAndDate()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        var food = await budget.ACategoryAsync(CategoryType.Expense);
        var rent = await budget.ACategoryAsync(CategoryType.Expense);
        var salary = await budget.ACategoryAsync(CategoryType.Gain);
        await budget.ATransactionAsync(food, new DateOnly(2026, 11, 1), 12.34m);
        await budget.ATransactionAsync(rent, new DateOnly(2026, 11, 1), 1000m);
        await budget.ATransactionAsync(food, new DateOnly(2026, 11, 2), 5.5m);
        await budget.ATransactionAsync(salary, new DateOnly(2026, 11, 1), 2500m);

        var result = await _repository.GetDailyTotalsAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo([
            new DailyTotal(CategoryType.Expense, new DateOnly(2026, 11, 1), 1012.34m),
            new DailyTotal(CategoryType.Expense, new DateOnly(2026, 11, 2), 5.5m),
            new DailyTotal(CategoryType.Gain, new DateOnly(2026, 11, 1), 2500m)
        ]);
    }

    [Fact]
    public async Task GetDailyTotalsAsync_LeavesOutSystemCategories()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        var transferOut = await budget.ACategoryAsync(CategoryType.Expense, system: true);
        var transferIn = await budget.ACategoryAsync(CategoryType.Gain, system: true);
        await budget.ATransactionAsync(transferOut, Today, 800.8m);
        await budget.ATransactionAsync(transferIn, Today, 7000.7m);

        var result = await _repository.GetDailyTotalsAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDailyTotalsAsync_LeavesOutOtherUsersTransactions()
    {
        var user = await AUser().SavedInDbAsync(database);
        var otherUser = await AUser().SavedInDbAsync(database);
        var otherBudget = new BudgetForTest(otherUser, database);
        await otherBudget.ATransactionAsync(await otherBudget.ACategoryAsync(CategoryType.Expense), Today, 42m);

        var result = await _repository.GetDailyTotalsAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDailyTotalsAsync_Fails_WhenDatabaseIsUnreachable()
    {
        var repository = new DashboardInDatabase(database.BadDataSource, new LoggerForTest<DashboardInDatabase>(testOutputHelper));

        var result = await repository.GetDailyTotalsAsync(AUser(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(DashboardErrors.ReadFailed);
    }

    [Fact]
    public async Task GetActiveAccountsAsync_TotalsTransactionsOfEachAccount()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        var chequing = await budget.AnAccountAsync("Chequing", AccountType.Checking, openBalance: 100m, openDate: new DateOnly(2025, 3, 1));
        var amex = await budget.AnAccountAsync("Amex", AccountType.CreditCard, openBalance: 25m, openDate: new DateOnly(2024, 6, 15));
        var food = await budget.ACategoryAsync(CategoryType.Expense);
        var salary = await budget.ACategoryAsync(CategoryType.Gain);
        var transferIn = await budget.ACategoryAsync(CategoryType.Gain, system: true);
        await budget.ATransactionAsync(salary, new DateOnly(2026, 11, 1), 2500m, chequing);
        await budget.ATransactionAsync(food, new DateOnly(2026, 11, 8), 613.59m, chequing);
        await budget.ATransactionAsync(food, new DateOnly(2026, 11, 3), 223.45m, amex);
        await budget.ATransactionAsync(transferIn, new DateOnly(2026, 11, 5), 100m, amex);

        var result = await _repository.GetActiveAccountsAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo([
            new AccountTotals(Id.Valid(amex), "Amex", AccountType.CreditCard, 25m, new DateOnly(2024, 6, 15), 100m, 223.45m, new DateOnly(2026, 11, 5)),
            new AccountTotals(Id.Valid(chequing), "Chequing", AccountType.Checking, 100m, new DateOnly(2025, 3, 1), 2500m, 613.59m, new DateOnly(2026, 11, 8))
        ], options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetActiveAccountsAsync_ReturnsZeroTotals_WhenAccountHasNoTransactions()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        var savings = await budget.AnAccountAsync("Savings", AccountType.Savings, openBalance: 12450m, openDate: new DateOnly(2025, 3, 1));

        var result = await _repository.GetActiveAccountsAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo([
            new AccountTotals(Id.Valid(savings), "Savings", AccountType.Savings, 12450m, new DateOnly(2025, 3, 1), 0m, 0m, null)
        ]);
    }

    [Fact]
    public async Task GetActiveAccountsAsync_ListsFavoritesFirstThenByName()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        await budget.AnAccountAsync("Savings", AccountType.Savings);
        await budget.AnAccountAsync("Visa", AccountType.CreditCard, favorite: true);
        await budget.AnAccountAsync("Amex", AccountType.CreditCard);
        await budget.AnAccountAsync("Chequing", AccountType.Checking, favorite: true);

        var result = await _repository.GetActiveAccountsAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Select(account => account.Name).Should().Equal("Chequing", "Visa", "Amex", "Savings");
    }

    [Fact]
    public async Task GetActiveAccountsAsync_LeavesOutInactiveAccounts()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        await budget.AnAccountAsync("Closed", AccountType.Checking, inactive: true);

        var result = await _repository.GetActiveAccountsAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetActiveAccountsAsync_LeavesOutOtherUsersAccounts()
    {
        var user = await AUser().SavedInDbAsync(database);
        var otherUser = await AUser().SavedInDbAsync(database);
        await new BudgetForTest(otherUser, database).AnAccountAsync("Chequing", AccountType.Checking);

        var result = await _repository.GetActiveAccountsAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetActiveAccountsAsync_Fails_WhenDatabaseIsUnreachable()
    {
        var repository = new DashboardInDatabase(database.BadDataSource, new LoggerForTest<DashboardInDatabase>(testOutputHelper));

        var result = await repository.GetActiveAccountsAsync(AUser(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(DashboardErrors.ReadFailed);
    }

    [Fact]
    public async Task GetRecentTransactionsAsync_ReturnsTheMostRecentTransactionsWithTheirCategory()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        var food = await budget.ACategoryAsync(CategoryType.Expense, name: "Food", color: "#FF8D28");
        var pay = await budget.ACategoryAsync(CategoryType.Gain, name: "Pay check", color: "#34C759");
        await budget.ATransactionAsync(food, new DateOnly(2026, 8, 26), 54.2m, description: "Gas");
        var lunch = await budget.ATransactionAsync(food, new DateOnly(2026, 9, 13), 1.25m, description: "Lunch", tags: "restaurant,work");
        var payCheck = await budget.ATransactionAsync(pay, new DateOnly(2026, 9, 12), 1000m);

        var result = await _repository.GetRecentTransactionsAsync(user, 2, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo([
            new RecentTransaction(Id.Valid(lunch), new DateOnly(2026, 9, 13), 1.25m, "Lunch", "Food", "#FF8D28", ["restaurant", "work"]),
            new RecentTransaction(Id.Valid(payCheck), new DateOnly(2026, 9, 12), 1000m, "", "Pay check", "#34C759", [])
        ], options => options.WithStrictOrdering());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" , ")]
    public async Task GetRecentTransactionsAsync_ReturnsNoTags_WhenTransactionHasNone(string? tags)
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        await budget.ATransactionAsync(await budget.ACategoryAsync(CategoryType.Expense), Today, 1m, tags: tags);

        var result = await _repository.GetRecentTransactionsAsync(user, 4, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().ContainSingle().Which.Tags.Should().BeEmpty();
    }

    [Fact]
    public async Task GetRecentTransactionsAsync_TrimsTags()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        await budget.ATransactionAsync(await budget.ACategoryAsync(CategoryType.Expense), Today, 1m, tags: " car , gas ");

        var result = await _repository.GetRecentTransactionsAsync(user, 4, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().ContainSingle().Which.Tags.Should().Equal("car", "gas");
    }

    [Fact]
    public async Task GetRecentTransactionsAsync_IncludesTransfers()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        await budget.ATransactionAsync(await budget.ACategoryAsync(CategoryType.Expense, system: true, name: "Transfer Out"), Today, 250m);

        var result = await _repository.GetRecentTransactionsAsync(user, 4, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().ContainSingle().Which.CategoryName.Should().Be("Transfer Out");
    }

    [Fact]
    public async Task GetRecentTransactionsAsync_LeavesOutOtherUsersTransactions()
    {
        var user = await AUser().SavedInDbAsync(database);
        var otherBudget = new BudgetForTest(await AUser().SavedInDbAsync(database), database);
        await otherBudget.ATransactionAsync(await otherBudget.ACategoryAsync(CategoryType.Expense), Today, 42m);

        var result = await _repository.GetRecentTransactionsAsync(user, 4, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetRecentTransactionsAsync_Fails_WhenDatabaseIsUnreachable()
    {
        var repository = new DashboardInDatabase(database.BadDataSource, new LoggerForTest<DashboardInDatabase>(testOutputHelper));

        var result = await repository.GetRecentTransactionsAsync(AUser(), 4, CancellationToken.None);

        result.Should().BeFailure().And.WithError(DashboardErrors.ReadFailed);
    }

    [Fact]
    public async Task GetUpcomingCashflowsAsync_ReturnsCashflowsWithTheirDetailsAndLastPayment()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        var housing = await budget.ACategoryAsync(CategoryType.Expense, name: "Housing", color: "#007AFF");
        var pay = await budget.ACategoryAsync(CategoryType.Gain, name: "Pay check", color: "#34C759");
        var rent = await budget.ACashflowAsync(housing, new DateOnly(2026, 1, 1), IntervalType.Monthly, 1, 1450m, description: "Rent", tags: " housing , home ");
        var salary = await budget.ACashflowAsync(pay, new DateOnly(2026, 1, 2), IntervalType.Weekly, 2, 2000m, tags: null);
        await budget.APaymentAsync(rent, housing, new DateOnly(2026, 11, 2), new DateOnly(2026, 11, 1), 1450m);
        var accountId = Id.Valid(await budget.TheAccountAsync());

        var result = await _repository.GetUpcomingCashflowsAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo([
            new UpcomingCashflow(
                Id.Valid(rent),
                new CashflowSchedule(accountId, new DateOnly(2026, 1, 1), IntervalType.Monthly, 1, 1450m, CategoryType.Expense,
                    new DateOnly(2026, 11, 2), new DateOnly(2026, 11, 1)),
                "Rent", "Housing", "#007AFF", ["housing", "home"]),
            new UpcomingCashflow(
                Id.Valid(salary),
                new CashflowSchedule(accountId, new DateOnly(2026, 1, 2), IntervalType.Weekly, 2, 2000m, CategoryType.Gain, null, null),
                "", "Pay check", "#34C759", [])
        ]);
    }

    [Fact]
    public async Task GetUpcomingCashflowsAsync_LeavesOutInactiveCashflows()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        await budget.ACashflowAsync(await budget.ACategoryAsync(CategoryType.Expense), Today, IntervalType.Monthly, 1, 10m, inactive: true);

        var result = await _repository.GetUpcomingCashflowsAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUpcomingCashflowsAsync_LeavesOutOtherUsersCashflows()
    {
        var user = await AUser().SavedInDbAsync(database);
        var otherBudget = new BudgetForTest(await AUser().SavedInDbAsync(database), database);
        await otherBudget.ACashflowAsync(await otherBudget.ACategoryAsync(CategoryType.Expense), Today, IntervalType.Monthly, 1, 10m);

        var result = await _repository.GetUpcomingCashflowsAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUpcomingCashflowsAsync_Fails_WhenDatabaseIsUnreachable()
    {
        var repository = new DashboardInDatabase(database.BadDataSource, new LoggerForTest<DashboardInDatabase>(testOutputHelper));

        var result = await repository.GetUpcomingCashflowsAsync(AUser(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(DashboardErrors.ReadFailed);
    }

    [Fact]
    public async Task GetActiveCashflowsAsync_ReturnsCashflowsWithTheirLastPayment()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        var rent = await budget.ACategoryAsync(CategoryType.Expense);
        var salary = await budget.ACategoryAsync(CategoryType.Gain);
        var rentFlow = await budget.ACashflowAsync(rent, new DateOnly(2026, 1, 1), IntervalType.Monthly, 1, 1450m);
        await budget.ACashflowAsync(salary, new DateOnly(2026, 1, 2), IntervalType.Weekly, 2, 2000m);
        await budget.APaymentAsync(rentFlow, rent, new DateOnly(2026, 9, 30), new DateOnly(2026, 10, 1), 1450m);
        await budget.APaymentAsync(rentFlow, rent, new DateOnly(2026, 11, 2), new DateOnly(2026, 11, 1), 1450m);
        var accountId = Id.Valid(await budget.TheAccountAsync());

        var result = await _repository.GetActiveCashflowsAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo([
            new CashflowSchedule(accountId, new DateOnly(2026, 1, 1), IntervalType.Monthly, 1, 1450m, CategoryType.Expense,
                new DateOnly(2026, 11, 2), new DateOnly(2026, 11, 1)),
            new CashflowSchedule(accountId, new DateOnly(2026, 1, 2), IntervalType.Weekly, 2, 2000m, CategoryType.Gain, null, null)
        ]);
    }

    [Fact]
    public async Task GetActiveCashflowsAsync_LeavesOutInactiveCashflows()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        await budget.ACashflowAsync(await budget.ACategoryAsync(CategoryType.Expense), Today, IntervalType.Monthly, 1, 50m, inactive: true);

        var result = await _repository.GetActiveCashflowsAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetActiveCashflowsAsync_LeavesOutOtherUsersCashflows()
    {
        var user = await AUser().SavedInDbAsync(database);
        var otherBudget = new BudgetForTest(await AUser().SavedInDbAsync(database), database);
        await otherBudget.ACashflowAsync(await otherBudget.ACategoryAsync(CategoryType.Expense), Today, IntervalType.Monthly, 1, 50m);

        var result = await _repository.GetActiveCashflowsAsync(user, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetActiveCashflowsAsync_Fails_WhenDatabaseIsUnreachable()
    {
        var repository = new DashboardInDatabase(database.BadDataSource, new LoggerForTest<DashboardInDatabase>(testOutputHelper));

        var result = await repository.GetActiveCashflowsAsync(AUser(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(DashboardErrors.ReadFailed);
    }

    [Fact]
    public async Task GetPeriodSettingsAsync_ReturnsDefault_WhenUserHasNoSettings()
    {
        var user = await AUser().SavedInDbAsync(database);

        var result = await _repository.GetPeriodSettingsAsync(user, Today, CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(PeriodSettings.Default(Today));
    }

    [Fact]
    public async Task GetPeriodSettingsAsync_ReadsCamelCaseSettings()
    {
        var user = await AUser().SavedInDbAsync(database);
        await new BudgetForTest(user, database).SettingsAsync(
            """{"effectiveDate":"2026-01-01","intervalType":"weekly","frequency":2}""");

        var result = await _repository.GetPeriodSettingsAsync(user, Today, CancellationToken.None);

        result.Should().BeSuccessful()
            .And.WithValue(PeriodSettings.Valid(new DateOnly(2026, 1, 1), IntervalType.Weekly, 2));
    }

    [Fact]
    public async Task GetPeriodSettingsAsync_ReadsPascalCaseSettings()
    {
        var user = await AUser().SavedInDbAsync(database);
        await new BudgetForTest(user, database).SettingsAsync(
            """{"EffectiveDate":"2025-01-01","IntervalType":"onetime","Frequency":1}""");

        var result = await _repository.GetPeriodSettingsAsync(user, Today, CancellationToken.None);

        result.Should().BeSuccessful()
            .And.WithValue(PeriodSettings.Valid(new DateOnly(2025, 1, 1), IntervalType.OneTime, 1));
    }

    [Fact]
    public async Task GetPeriodSettingsAsync_LeavesOutOtherUsersSettings()
    {
        var user = await AUser().SavedInDbAsync(database);
        var otherUser = await AUser().SavedInDbAsync(database);
        await new BudgetForTest(otherUser, database).SettingsAsync(
            """{"effectiveDate":"2026-01-01","intervalType":"weekly","frequency":2}""");

        var result = await _repository.GetPeriodSettingsAsync(user, Today, CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(PeriodSettings.Default(Today));
    }

    [Theory]
    [InlineData("not json")]
    [InlineData("""{"intervalType":"monthly","frequency":1}""")]
    [InlineData("""{"effectiveDate":"2026-01-01","intervalType":"fortnightly","frequency":1}""")]
    [InlineData("""{"effectiveDate":"2026-01-01","intervalType":"monthly","frequency":0}""")]
    [InlineData("""{"effectiveDate":"2026-01-01","intervalType":"monthly"}""")]
    public async Task GetPeriodSettingsAsync_Fails_WhenSettingsAreInvalid(string data)
    {
        var user = await AUser().SavedInDbAsync(database);
        await new BudgetForTest(user, database).SettingsAsync(data);

        var result = await _repository.GetPeriodSettingsAsync(user, Today, CancellationToken.None);

        result.Should().BeFailure().And.WithError(DashboardErrors.SettingsInvalid);
    }

    [Fact]
    public async Task GetPeriodSettingsAsync_Fails_WhenDatabaseIsUnreachable()
    {
        var repository = new DashboardInDatabase(database.BadDataSource, new LoggerForTest<DashboardInDatabase>(testOutputHelper));

        var result = await repository.GetPeriodSettingsAsync(AUser(), Today, CancellationToken.None);

        result.Should().BeFailure().And.WithError(DashboardErrors.ReadFailed);
    }
}
