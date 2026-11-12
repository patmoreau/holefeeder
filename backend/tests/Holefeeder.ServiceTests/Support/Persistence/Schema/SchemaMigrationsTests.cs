using Dapper;

using Holefeeder.Service.Support.Persistence.Schema;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.Support.Persistence.Schema;

[Collection(DatabaseTestMarker.Name)]
public class SchemaMigrationsTests(DatabaseForTest database, ITestOutputHelper testOutputHelper)
{
    private const string ScriptsInJournal = "SELECT scriptname FROM schemaversions ORDER BY schemaversionsid;";

    private static readonly string[] JournaledScripts =
    [
        "Holefeeder.Infrastructure.Scripts.001-CreateAccountsTable.sql",
        "Holefeeder.Infrastructure.Scripts.002-CreateCategoriesTable.sql",
        "Holefeeder.Infrastructure.Scripts.003-CreateCashflowsTable.sql",
        "Holefeeder.Infrastructure.Scripts.004-CreateTransactionsTable.sql",
        "Holefeeder.Infrastructure.Scripts.005-CreateStoreItemsTable.sql",
        "Holefeeder.Infrastructure.Scripts.006-FixTransactionDescriptions.sql",
        "Holefeeder.Infrastructure.Scripts.007-CreateUsersTable.sql",
        "Holefeeder.Infrastructure.Scripts.008-FixTagsToLower.sql",
        "Holefeeder.Infrastructure.Scripts.009-AddUsersForeignKeys.sql",
        "Holefeeder.Infrastructure.Scripts.010-AddInactiveToCategories.sql"
    ];

    [Fact]
    public async Task MigrateDb_JournalsEachScriptUnderTheNameProductionDatabasesHold()
    {
        var scripts = await ScriptsInJournalAsync();

        scripts.Should().Equal(JournaledScripts);
    }

    [Fact]
    public async Task MigrateDb_AppliesNothing_WhenEveryScriptIsJournaled()
    {
        SchemaMigrations.MigrateDb(database.ConnectionString, new LoggerForTest<SchemaMigrationsTests>(testOutputHelper));

        var scripts = await ScriptsInJournalAsync();

        scripts.Should().Equal(JournaledScripts);
    }

    private async Task<IReadOnlyList<string>> ScriptsInJournalAsync()
    {
        await using var connection = database.DataSource.CreateConnection();
        return (await connection.QueryAsync<string>(ScriptsInJournal)).ToList();
    }
}
