using System.Data;
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

        try
        {
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
        catch (MySqlException e)
        {
            if (e.ErrorCode is not (MySqlErrorCode.UnableToConnectToHost or MySqlErrorCode.UnknownDatabase))
            {
                Console.WriteLine(e);
                throw;
            }

            if (e.InnerException is not null && e.InnerException is not MySqlException {ErrorCode: MySqlErrorCode.UnknownDatabase})
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public async Task ResetCheckpoint()
    {
        if (_connection.State is ConnectionState.Closed or ConnectionState.Broken)
        {
            await this.InitializeAsync();
        }
        await _respawner.ResetAsync(_connection);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await _connection.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
