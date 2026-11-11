using Dapper;

using Holefeeder.Service.Settings.Persistence;
using Holefeeder.Service.Shared.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.Settings.Persistence;

[Collection(DatabaseTestMarker.Name)]
public class SettingsInDatabaseTests(DatabaseForTest database, ITestOutputHelper testOutputHelper)
{
    private static readonly DateOnly Today = new(2026, 11, 11);

    private readonly SettingsInDatabase _repository = new(database.DataSource, new LoggerForTest<SettingsInDatabase>(testOutputHelper));

    [Fact]
    public async Task GetAsync_ReturnsMonthlyFromToday_WhenTheUserHasNoSettings()
    {
        var user = await AUser().SavedInDbAsync(database);

        var result = await _repository.GetAsync(user, Today, CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(PeriodSettings.Default(Today));
    }

    [Fact]
    public async Task GetAsync_ReadsTheStoredSettings()
    {
        var user = await AUser().SavedInDbAsync(database);
        await new BudgetForTest(user, database).SettingsAsync("""{"effectiveDate":"2026-09-01","intervalType":"weekly","frequency":2}""");

        var result = await _repository.GetAsync(user, Today, CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(PeriodSettings.Valid(new DateOnly(2026, 9, 1), IntervalType.Weekly, 2));
    }

    [Fact]
    public async Task GetAsync_Fails_WhenTheStoredSettingsAreUnreadable()
    {
        var user = await AUser().SavedInDbAsync(database);
        await new BudgetForTest(user, database).SettingsAsync("""{"effectiveDate":"soon"}""");

        var result = await _repository.GetAsync(user, Today, CancellationToken.None);

        result.Should().BeFailure().And.WithError(SettingsErrors.Invalid);
    }

    [Fact]
    public async Task GetAsync_Fails_WhenTheDatabaseIsUnreachable()
    {
        var repository = new SettingsInDatabase(database.BadDataSource, new LoggerForTest<SettingsInDatabase>(testOutputHelper));

        var result = await repository.GetAsync(AUser(), Today, CancellationToken.None);

        result.Should().BeFailure().And.WithError(SettingsErrors.ReadFailed);
    }

    [Fact]
    public async Task SaveAsync_StoresTheSettingsAsTheSettingsStoreItem()
    {
        var user = await AUser().SavedInDbAsync(database);
        var settings = PeriodSettings.Valid(new DateOnly(2026, 9, 1), IntervalType.OneTime, 3);

        var result = await _repository.SaveAsync(user, settings, CancellationToken.None);

        result.Should().BeSuccessful();
        (await StoredDataAsync(user)).Should().Equal("""{"effectiveDate":"2026-09-01","intervalType":"oneTime","frequency":3}""");
        (await _repository.GetAsync(user, Today, CancellationToken.None)).Should().BeSuccessful().And.WithValue(settings);
    }

    [Fact]
    public async Task SaveAsync_ReplacesTheExistingSettings()
    {
        var user = await AUser().SavedInDbAsync(database);
        await new BudgetForTest(user, database).SettingsAsync("""{"effectiveDate":"2025-01-01","intervalType":"monthly","frequency":1}""");

        var result = await _repository.SaveAsync(user, PeriodSettings.Valid(new DateOnly(2026, 9, 1), IntervalType.Weekly, 2), CancellationToken.None);

        result.Should().BeSuccessful();
        (await StoredDataAsync(user)).Should().Equal("""{"effectiveDate":"2026-09-01","intervalType":"weekly","frequency":2}""");
    }

    [Fact]
    public async Task SaveAsync_LeavesOtherUsersSettingsAlone()
    {
        var user = await AUser().SavedInDbAsync(database);
        var otherUser = await AUser().SavedInDbAsync(database);
        await new BudgetForTest(otherUser, database).SettingsAsync("""{"effectiveDate":"2025-01-01","intervalType":"monthly","frequency":1}""");

        await _repository.SaveAsync(user, PeriodSettings.Valid(new DateOnly(2026, 9, 1), IntervalType.Weekly, 2), CancellationToken.None);

        (await StoredDataAsync(otherUser)).Should().Equal("""{"effectiveDate":"2025-01-01","intervalType":"monthly","frequency":1}""");
    }

    [Fact]
    public async Task SaveAsync_Fails_WhenTheDatabaseIsUnreachable()
    {
        var repository = new SettingsInDatabase(database.BadDataSource, new LoggerForTest<SettingsInDatabase>(testOutputHelper));

        var result = await repository.SaveAsync(AUser(), PeriodSettings.Default(Today), CancellationToken.None);

        result.Should().BeFailure().And.WithError(SettingsErrors.WriteFailed);
    }

    private async Task<string[]> StoredDataAsync(Holefeeder.Service.Support.Core.User user)
    {
        await using var connection = await database.DataSource.OpenConnectionAsync();
        var rows = await connection.QueryAsync<string>(
            "SELECT data FROM store_items WHERE user_id = @UserId AND code = 'settings';", new { UserId = user.Id.Value });
        return [.. rows];
    }
}
