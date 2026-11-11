using Holefeeder.Service.Dashboard.Core;
using Holefeeder.Service.Dashboard.Core.GetDashboardSummary;
using Holefeeder.Service.Dashboard.Persistence;
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
