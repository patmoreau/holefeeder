using Holefeeder.Service.Accounts.Core;
using Holefeeder.Service.Accounts.Core.GetAccountDetail;
using Holefeeder.Service.Accounts.Core.GetAccountTransactions;
using Holefeeder.Service.Accounts.Persistence;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Shared.Core.Search;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.Accounts.Persistence;

[Collection(DatabaseTestMarker.Name)]
public class AccountsInDatabaseTests(DatabaseForTest database, ITestOutputHelper testOutputHelper)
{
    private static readonly DateOnly Today = new(2026, 11, 10);

    private readonly AccountsInDatabase _repository = new(database.DataSource, new LoggerForTest<AccountsInDatabase>(testOutputHelper));

    [Fact]
    public async Task GetAccountTotalsAsync_TotalsTheTransactionsOfTheAccount()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        var chequing = await budget.AnAccountAsync("Chequing", AccountType.Checking, openBalance: 100m, openDate: new DateOnly(2025, 3, 1),
            favorite: true, description: "Main bank account");
        var amex = await budget.AnAccountAsync("Amex", AccountType.CreditCard);
        var food = await budget.ACategoryAsync(CategoryType.Expense);
        var salary = await budget.ACategoryAsync(CategoryType.Gain);
        var transferIn = await budget.ACategoryAsync(CategoryType.Gain, system: true);
        await budget.ATransactionAsync(salary, new DateOnly(2026, 11, 1), 2500m, chequing);
        await budget.ATransactionAsync(transferIn, new DateOnly(2026, 11, 2), 200m, chequing);
        await budget.ATransactionAsync(food, new DateOnly(2026, 11, 8), 613.59m, chequing);
        await budget.ATransactionAsync(food, new DateOnly(2026, 11, 9), 223.45m, amex);

        var result = await _repository.GetAccountTotalsAsync(user, Id.Valid(chequing), CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(new AccountTotals(
            Id.Valid(chequing), "Chequing", AccountType.Checking, "Main bank account", 100m, new DateOnly(2025, 3, 1),
            Favorite: true, Inactive: false, Gains: 2700m, Expenses: 613.59m, LastTransactionDate: new DateOnly(2026, 11, 8)));
    }

    [Fact]
    public async Task GetAccountTotalsAsync_ReturnsZeroTotals_WhenTheAccountHasNoTransactions()
    {
        var user = await AUser().SavedInDbAsync(database);
        var savings = await new BudgetForTest(user, database).AnAccountAsync("Savings", AccountType.Savings, openBalance: 12450m,
            openDate: new DateOnly(2025, 3, 1), inactive: true);

        var result = await _repository.GetAccountTotalsAsync(user, Id.Valid(savings), CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(new AccountTotals(
            Id.Valid(savings), "Savings", AccountType.Savings, string.Empty, 12450m, new DateOnly(2025, 3, 1),
            Favorite: false, Inactive: true, Gains: 0m, Expenses: 0m, LastTransactionDate: null));
    }

    [Fact]
    public async Task GetAccountTotalsAsync_ReturnsNotFound_WhenTheAccountBelongsToAnotherUser()
    {
        var user = await AUser().SavedInDbAsync(database);
        var otherUser = await AUser().SavedInDbAsync(database);
        var account = await new BudgetForTest(otherUser, database).AnAccountAsync("Chequing", AccountType.Checking);

        var result = await _repository.GetAccountTotalsAsync(user, Id.Valid(account), CancellationToken.None);

        result.Should().BeFailure().And.WithError(AccountErrors.NotFound);
    }

    [Fact]
    public async Task GetAccountTotalsAsync_ReturnsNotFound_WhenTheAccountDoesNotExist()
    {
        var user = await AUser().SavedInDbAsync(database);

        var result = await _repository.GetAccountTotalsAsync(user, AnId(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(AccountErrors.NotFound);
    }

    [Fact]
    public async Task GetAccountTotalsAsync_Fails_WhenDatabaseIsUnreachable()
    {
        var repository = new AccountsInDatabase(database.BadDataSource, new LoggerForTest<AccountsInDatabase>(testOutputHelper));

        var result = await repository.GetAccountTotalsAsync(AUser(), AnId(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(AccountsErrors.ReadFailed);
    }

    [Fact]
    public async Task GetActiveCashflowsAsync_ReturnsTheActiveCashflowsOfTheAccountWithTheirLastPayment()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        var accountId = await budget.TheAccountAsync();
        var otherAccount = await budget.AnAccountAsync("Amex", AccountType.CreditCard);
        var rent = await budget.ACategoryAsync(CategoryType.Expense);
        var salary = await budget.ACategoryAsync(CategoryType.Gain);
        var rentFlow = await budget.ACashflowAsync(rent, new DateOnly(2026, 1, 1), IntervalType.Monthly, 1, 1450m);
        await budget.ACashflowAsync(salary, new DateOnly(2026, 1, 2), IntervalType.Weekly, 2, 2000m);
        await budget.ACashflowAsync(rent, new DateOnly(2026, 1, 3), IntervalType.Monthly, 1, 75m, inactive: true);
        await budget.ACashflowAsync(rent, new DateOnly(2026, 1, 4), IntervalType.Monthly, 1, 40m, accountId: otherAccount);
        await budget.APaymentAsync(rentFlow, rent, new DateOnly(2026, 11, 2), new DateOnly(2026, 11, 1), 1450m);

        var result = await _repository.GetActiveCashflowsAsync(user, Id.Valid(accountId), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo([
            new CashflowSchedule(Id.Valid(accountId), new DateOnly(2026, 1, 1), IntervalType.Monthly, 1, 1450m, CategoryType.Expense,
                new DateOnly(2026, 11, 2), new DateOnly(2026, 11, 1)),
            new CashflowSchedule(Id.Valid(accountId), new DateOnly(2026, 1, 2), IntervalType.Weekly, 2, 2000m, CategoryType.Gain, null, null)
        ]);
    }

    [Fact]
    public async Task GetActiveCashflowsAsync_LeavesOutOtherUsersCashflows()
    {
        var user = await AUser().SavedInDbAsync(database);
        var otherBudget = new BudgetForTest(await AUser().SavedInDbAsync(database), database);
        var otherAccount = await otherBudget.TheAccountAsync();
        await otherBudget.ACashflowAsync(await otherBudget.ACategoryAsync(CategoryType.Expense), Today, IntervalType.Monthly, 1, 50m);

        var result = await _repository.GetActiveCashflowsAsync(user, Id.Valid(otherAccount), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetActiveCashflowsAsync_Fails_WhenDatabaseIsUnreachable()
    {
        var repository = new AccountsInDatabase(database.BadDataSource, new LoggerForTest<AccountsInDatabase>(testOutputHelper));

        var result = await repository.GetActiveCashflowsAsync(AUser(), AnId(), CancellationToken.None);

        result.Should().BeFailure().And.WithError(AccountsErrors.ReadFailed);
    }

    [Fact]
    public async Task GetPeriodSettingsAsync_ReturnsDefault_WhenUserHasNoSettings()
    {
        var user = await AUser().SavedInDbAsync(database);

        var result = await _repository.GetPeriodSettingsAsync(user, Today, CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(PeriodSettings.Default(Today));
    }

    [Fact]
    public async Task GetPeriodSettingsAsync_ReadsTheUsersSettings()
    {
        var user = await AUser().SavedInDbAsync(database);
        await new BudgetForTest(user, database).SettingsAsync(
            """{"effectiveDate":"2026-01-01","intervalType":"weekly","frequency":2}""");

        var result = await _repository.GetPeriodSettingsAsync(user, Today, CancellationToken.None);

        result.Should().BeSuccessful()
            .And.WithValue(PeriodSettings.Valid(new DateOnly(2026, 1, 1), IntervalType.Weekly, 2));
    }

    [Fact]
    public async Task GetPeriodSettingsAsync_Fails_WhenSettingsAreInvalid()
    {
        var user = await AUser().SavedInDbAsync(database);
        await new BudgetForTest(user, database).SettingsAsync("not json");

        var result = await _repository.GetPeriodSettingsAsync(user, Today, CancellationToken.None);

        result.Should().BeFailure().And.WithError(AccountsErrors.SettingsInvalid);
    }

    [Fact]
    public async Task GetPeriodSettingsAsync_Fails_WhenDatabaseIsUnreachable()
    {
        var repository = new AccountsInDatabase(database.BadDataSource, new LoggerForTest<AccountsInDatabase>(testOutputHelper));

        var result = await repository.GetPeriodSettingsAsync(AUser(), Today, CancellationToken.None);

        result.Should().BeFailure().And.WithError(AccountsErrors.ReadFailed);
    }

    [Fact]
    public async Task GetTransactionsAsync_ReturnsTheAccountsTransactionsNewestFirst()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        var chequing = await budget.AnAccountAsync("Chequing", AccountType.Checking);
        var amex = await budget.AnAccountAsync("Amex", AccountType.CreditCard);
        var food = await budget.ACategoryAsync(CategoryType.Expense, name: "Food", color: "#FF8D28");
        var pay = await budget.ACategoryAsync(CategoryType.Gain, name: "Pay check", color: "#34C759");
        var gas = await budget.ATransactionAsync(food, new DateOnly(2026, 8, 26), 54.2m, chequing, description: "Gas", tags: null);
        var lunch = await budget.ATransactionAsync(food, new DateOnly(2026, 9, 13), 1.25m, chequing, description: "Lunch", tags: "restaurant, work");
        var payCheck = await budget.ATransactionAsync(pay, new DateOnly(2026, 9, 12), 1000m, chequing);
        await budget.ATransactionAsync(food, new DateOnly(2026, 9, 14), 9.99m, amex);

        var result = await _repository.GetTransactionsAsync(user, Id.Valid(chequing), Offset.Default, Limit.Default, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Total.Should().Be(3);
        result.Value.Items.Should().BeEquivalentTo([
            new RecordedTransaction(Id.Valid(lunch), Id.Valid(chequing), new DateOnly(2026, 9, 13), 1.25m, "Lunch", "Food", "#FF8D28", ["restaurant", "work"]),
            new RecordedTransaction(Id.Valid(payCheck), Id.Valid(chequing), new DateOnly(2026, 9, 12), 1000m, "", "Pay check", "#34C759", []),
            new RecordedTransaction(Id.Valid(gas), Id.Valid(chequing), new DateOnly(2026, 8, 26), 54.2m, "Gas", "Food", "#FF8D28", [])
        ], options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetTransactionsAsync_ReturnsTheRequestedPageWithTheTotal()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        var chequing = await budget.TheAccountAsync();
        var food = await budget.ACategoryAsync(CategoryType.Expense);
        for (var day = 1; day <= 5; day++)
        {
            await budget.ATransactionAsync(food, new DateOnly(2026, 9, day), day, chequing);
        }

        var result = await _repository.GetTransactionsAsync(user, Id.Valid(chequing), Offset.Valid(2), Limit.Valid(2), CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Total.Should().Be(5);
        result.Value.Items.Select(transaction => transaction.Amount).Should().Equal(3m, 2m);
    }

    [Fact]
    public async Task GetTransactionsAsync_ReturnsNoTransaction_WhenTheAccountHasNone()
    {
        var user = await AUser().SavedInDbAsync(database);
        var savings = await new BudgetForTest(user, database).AnAccountAsync("Savings", AccountType.Savings);

        var result = await _repository.GetTransactionsAsync(user, Id.Valid(savings), Offset.Default, Limit.Default, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Total.Should().Be(0);
        result.Value.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTransactionsAsync_ReturnsNotFound_WhenTheAccountBelongsToAnotherUser()
    {
        var user = await AUser().SavedInDbAsync(database);
        var otherBudget = new BudgetForTest(await AUser().SavedInDbAsync(database), database);
        var otherAccount = await otherBudget.TheAccountAsync();
        await otherBudget.ATransactionAsync(await otherBudget.ACategoryAsync(CategoryType.Expense), Today, 10m, otherAccount);

        var result = await _repository.GetTransactionsAsync(user, Id.Valid(otherAccount), Offset.Default, Limit.Default, CancellationToken.None);

        result.Should().BeFailure().And.WithError(AccountErrors.NotFound);
    }

    [Fact]
    public async Task GetTransactionsAsync_Fails_WhenDatabaseIsUnreachable()
    {
        var repository = new AccountsInDatabase(database.BadDataSource, new LoggerForTest<AccountsInDatabase>(testOutputHelper));

        var result = await repository.GetTransactionsAsync(AUser(), AnId(), Offset.Default, Limit.Default, CancellationToken.None);

        result.Should().BeFailure().And.WithError(AccountsErrors.ReadFailed);
    }
    [Fact]
    public async Task SaveAsync_InsertsTheAccount_WhenItDoesNotExist()
    {
        var user = await AUser().SavedInDbAsync(database);
        var account = AnAccount("Amex") with { Type = AccountType.CreditCard, OpenBalance = Money.Valid(250.75m), Description = "Travel card" };

        var result = await _repository.SaveAsync(user, account, CancellationToken.None);

        result.Should().BeSuccessful();
        (await _repository.GetAccountTotalsAsync(user, account.Id, CancellationToken.None)).Should().BeSuccessful().And.WithValue(
            new AccountTotals(account.Id, "Amex", AccountType.CreditCard, "Travel card", 250.75m, new DateOnly(2025, 3, 1),
                Favorite: false, Inactive: false, Gains: 0m, Expenses: 0m, LastTransactionDate: null));
    }

    [Fact]
    public async Task SaveAsync_ReplacesTheAccount_WhenTheUserOwnsIt()
    {
        var user = await AUser().SavedInDbAsync(database);
        var account = AnAccount("Chequing");
        var other = AnAccount("Savings");
        await _repository.SaveAsync(user, account, CancellationToken.None);
        await _repository.SaveAsync(user, other, CancellationToken.None);
        var modified = account with
        {
            Name = AccountName.Valid("Main chequing"),
            OpenBalance = Money.Valid(10m),
            OpenDate = new DateOnly(2024, 1, 1),
            Description = "Renamed",
            Favorite = true,
            Inactive = true
        };

        var result = await _repository.SaveAsync(user, modified, CancellationToken.None);

        result.Should().BeSuccessful();
        (await _repository.GetAccountTotalsAsync(user, account.Id, CancellationToken.None)).Should().BeSuccessful().And.WithValue(
            new AccountTotals(account.Id, "Main chequing", AccountType.Checking, "Renamed", 10m, new DateOnly(2024, 1, 1),
                Favorite: true, Inactive: true, Gains: 0m, Expenses: 0m, LastTransactionDate: null));
        (await _repository.GetAccountTotalsAsync(user, other.Id, CancellationToken.None)).Should().BeSuccessful();
        (await _repository.GetAccountTotalsAsync(user, other.Id, CancellationToken.None)).Value.Name.Should().Be("Savings");
    }

    [Fact]
    public async Task SaveAsync_FailsWithNotFound_WhenTheAccountBelongsToAnotherUser()
    {
        var owner = await AUser().SavedInDbAsync(database);
        var account = AnAccount("Chequing");
        await _repository.SaveAsync(owner, account, CancellationToken.None);
        var user = await AUser().SavedInDbAsync(database);

        var result = await _repository.SaveAsync(user, account with { Name = AccountName.Valid("Taken over") }, CancellationToken.None);

        result.Should().BeFailure().And.WithError(AccountErrors.NotFound);
        (await _repository.GetAccountTotalsAsync(owner, account.Id, CancellationToken.None)).Value.Name.Should().Be("Chequing");
    }

    [Fact]
    public async Task SaveAsync_FailsWithNameAlreadyExists_WhenTheUserHasTheName()
    {
        var user = await AUser().SavedInDbAsync(database);
        await new BudgetForTest(user, database).AnAccountAsync("Chequing", AccountType.Checking);

        var result = await _repository.SaveAsync(user, AnAccount("Chequing"), CancellationToken.None);

        result.Should().BeFailure().And.WithError(AccountNameErrors.AlreadyExists);
    }

    [Fact]
    public async Task SaveAsync_InsertsTheAccount_WhenAnotherUserHasTheName()
    {
        var user = await AUser().SavedInDbAsync(database);
        await new BudgetForTest(await AUser().SavedInDbAsync(database), database).AnAccountAsync("Chequing", AccountType.Checking);

        var result = await _repository.SaveAsync(user, AnAccount("Chequing"), CancellationToken.None);

        result.Should().BeSuccessful();
    }

    [Fact]
    public async Task SaveAsync_Fails_WhenDatabaseIsUnreachable()
    {
        var repository = new AccountsInDatabase(database.BadDataSource, new LoggerForTest<AccountsInDatabase>(testOutputHelper));

        var result = await repository.SaveAsync(AUser(), AnAccount("Chequing"), CancellationToken.None);

        result.Should().BeFailure().And.WithError(AccountsErrors.WriteFailed);
    }

    private static Account AnAccount(string name) =>
        Account.Open(AnId(), AccountType.Checking, AccountName.Valid(name), new DateOnly(2025, 3, 1), Money.Zero, string.Empty);
}
