using Holefeeder.Infrastructure.Context;
using Holefeeder.Infrastructure.Entities;
using Holefeeder.Infrastructure.Extensions;

using Microsoft.Extensions.Options;

using MySqlConnector;

using Respawn;

using TechTalk.SpecFlow.Assist;

namespace Holefeeder.FunctionalTests.Drivers;

[Binding]
public class DatabaseDriver
{
    private readonly ObjectStoreDatabaseSettings _settings;

    public DatabaseDriver(IOptions<ObjectStoreDatabaseSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task ResetState()
    {
        var checkpoint = new Checkpoint
        {
            SchemasToInclude = new[] {"object_store_functional_tests"},
            DbAdapter = DbAdapter.MySql,
            TablesToInclude = new Respawn.Graph.Table[] {"store_items"},
            TablesToIgnore = new Respawn.Graph.Table[] {"schema_versions"}
        };

        await using var connection = new MySqlConnection(_settings.ConnectionString);

        await connection.OpenAsync();

        await checkpoint.Reset(connection);
    }

    public async Task AddAccountsToDatabase(Table items)
    {
        await using var connection = new MySqlConnection(_settings.ConnectionString);

        var rows = items.CreateSet<AccountEntity>();

        foreach (var row in rows)
        {
            await connection.InsertAsync(row);
        }
    }

    public async Task AddStoreItemsToDatabase(Table items)
    {
        await using var connection = new MySqlConnection(_settings.ConnectionString);

        var rows = items.CreateSet<StoreItemEntity>();

        foreach (var row in rows)
        {
            await connection.InsertAsync(row);
        }
    }
}
