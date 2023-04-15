using System.Data.Common;
using Holefeeder.Infrastructure.SeedWork;
using Holefeeder.Tests.Common.SeedWork.Drivers;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Respawn;
using Respawn.Graph;

namespace Holefeeder.FunctionalTests.Drivers;

public class BudgetingDatabaseInitializer : DatabaseInitializer
{
    private readonly string _connectionString;
    private DbConnection? _connection;

    public BudgetingDatabaseInitializer()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.tests.json"))
            .AddUserSecrets<BudgetingDatabaseInitializer>()
            .AddEnvironmentVariables()
            .Build();
        _connectionString =
            configuration.GetConnectionString(BudgetingConnectionStringBuilder.BUDGETING_CONNECTION_STRING)!;
    }

    protected override DbConnection Connection
    {
        get { return _connection ??= new MySqlConnection(_connectionString); }
    }

    protected override RespawnerOptions Options { get; } = new()
    {
        DbAdapter = DbAdapter.MySql,
        SchemasToInclude = new[] { "budgeting_functional_tests" },
        TablesToInclude =
            new Table[] { "accounts", "cashflows", "categories", "store_items", "transactions" },
        TablesToIgnore = new Table[] { "schema_versions" },
        WithReseed = true
    };

    public override async Task InitializeAsync()
    {
        try
        {
            await base.InitializeAsync();
        }
        catch (MySqlException e)
        {
            if (e.ErrorCode is not (MySqlErrorCode.UnableToConnectToHost or MySqlErrorCode.UnknownDatabase))
            {
                Console.WriteLine(e);
                throw;
            }

            if (e.InnerException is not null && e.InnerException is not MySqlException
                {
                    ErrorCode: MySqlErrorCode.UnknownDatabase
                })
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
