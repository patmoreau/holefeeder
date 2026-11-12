using Holefeeder.Service.Insights.Core;
using Holefeeder.Service.Insights.Persistence;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.Insights.Persistence;

[Collection(DatabaseTestMarker.Name)]
public class InsightsInDatabaseTests(DatabaseForTest database, ITestOutputHelper testOutputHelper)
{
    private static readonly DateOnly Today = new(2026, 11, 10);

    private readonly InsightsInDatabase _repository = new(database.DataSource, new LoggerForTest<InsightsInDatabase>(testOutputHelper));

    [Fact]
    public async Task GetPeriodSettingsAsync_ReadsTheUsersSettings()
    {
        var user = await AUser().SavedInDbAsync(database);
        await new BudgetForTest(user, database).SettingsAsync("""{"effectiveDate":"2025-01-03","intervalType":"weekly","frequency":2}""");

        var result = await _repository.GetPeriodSettingsAsync(user, Today, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().Be(PeriodSettings.Valid(new DateOnly(2025, 1, 3), IntervalType.Weekly, 2));
    }

    [Fact]
    public async Task GetPeriodSettingsAsync_ReturnsTheDefault_WhenTheUserHasNoSettings()
    {
        var user = await AUser().SavedInDbAsync(database);

        var result = await _repository.GetPeriodSettingsAsync(user, Today, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().Be(PeriodSettings.Default(Today));
    }

    [Fact]
    public async Task GetCategoriesAsync_ReturnsTheUsersNonSystemCategoriesOfTheType()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        var food = await budget.ACategoryAsync(CategoryType.Expense, name: "Food", color: "#34C759", budget: 450.5m);
        await budget.ACategoryAsync(CategoryType.Expense, system: true, name: "Transfer");
        await budget.ACategoryAsync(CategoryType.Gain, name: "Salary");
        var otherBudget = new BudgetForTest(await AUser().SavedInDbAsync(database), database);
        await otherBudget.ACategoryAsync(CategoryType.Expense, name: "Theirs");

        var result = await _repository.GetCategoriesAsync(user, CategoryType.Expense, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().Equal(new InsightCategory(Id.Valid(food), "Food", "#34C759", 450.5m));
    }

    [Fact]
    public async Task GetEntriesAsync_ReturnsTheUsersTransactionsInNonSystemCategoriesOfTheType()
    {
        var user = await AUser().SavedInDbAsync(database);
        var budget = new BudgetForTest(user, database);
        var food = await budget.ACategoryAsync(CategoryType.Expense);
        var transfer = await budget.ACategoryAsync(CategoryType.Expense, system: true);
        var salary = await budget.ACategoryAsync(CategoryType.Gain);
        await budget.ATransactionAsync(food, Today, 12.34m, tags: "costco,weekly");
        await budget.ATransactionAsync(food, Today.AddDays(-1), 5m, tags: null);
        await budget.ATransactionAsync(transfer, Today, 800m);
        await budget.ATransactionAsync(salary, Today, 2500m);
        var otherBudget = new BudgetForTest(await AUser().SavedInDbAsync(database), database);
        await otherBudget.ATransactionAsync(await otherBudget.ACategoryAsync(CategoryType.Expense), Today, 42m);

        var result = await _repository.GetEntriesAsync(user, CategoryType.Expense, CancellationToken.None);

        result.Should().BeSuccessful();
        result.Value.Should().BeEquivalentTo([
            new InsightEntry(Today, Id.Valid(food), 12.34m, "costco,weekly"),
            new InsightEntry(Today.AddDays(-1), Id.Valid(food), 5m, "")
        ]);
    }

    [Fact]
    public async Task GetPeriodSettingsAsync_Fails_WhenDatabaseIsUnreachable()
    {
        var result = await Unreachable().GetPeriodSettingsAsync(AUser(), Today, CancellationToken.None);

        result.Should().BeFailure().And.WithError(InsightsErrors.ReadFailed);
    }

    [Fact]
    public async Task GetCategoriesAsync_Fails_WhenDatabaseIsUnreachable()
    {
        var result = await Unreachable().GetCategoriesAsync(AUser(), CategoryType.Expense, CancellationToken.None);

        result.Should().BeFailure().And.WithError(InsightsErrors.ReadFailed);
    }

    [Fact]
    public async Task GetEntriesAsync_Fails_WhenDatabaseIsUnreachable()
    {
        var result = await Unreachable().GetEntriesAsync(AUser(), CategoryType.Expense, CancellationToken.None);

        result.Should().BeFailure().And.WithError(InsightsErrors.ReadFailed);
    }

    private InsightsInDatabase Unreachable() =>
        new(database.BadDataSource, new LoggerForTest<InsightsInDatabase>(testOutputHelper));
}
