using System.Data.Common;

using Holefeeder.Infrastructure.SeedWork;

using Microsoft.Extensions.Configuration;

using MySqlConnector;

using Respawn;
using Respawn.Graph;

namespace Holefeeder.FunctionalTests.Drivers;

public class BudgetingDatabaseInitializer : IAsyncLifetime, IAsyncDisposable
{
    private readonly string _connectionString;
    private Respawner _respawner = default!;
    private DbConnection _connection = default!;

    public BudgetingDatabaseInitializer()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.tests.json"))
            .AddUserSecrets<BudgetingDatabaseInitializer>()
            .AddEnvironmentVariables()
            .Build();
        _connectionString =
            configuration.GetConnectionString(BudgetingConnectionStringBuilder.BUDGETING_CONNECTION_STRING)!;
    }

    public async Task InitializeAsync()
    {
        _connection = new MySqlConnection(_connectionString);

        await _connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_connection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.MySql,
                SchemasToInclude = new[] {"budgeting_functional_tests"},
                TablesToInclude =
                    new Table[] {"accounts", "cashflows", "categories", "store_items", "transactions"},
                TablesToIgnore = new Table[] {"schema_versions"},
                WithReseed = true
            });
    }

    public async Task ResetCheckpoint()
    {
        await _respawner.ResetAsync(_connection);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}
