using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.Infrastructure.Persistence;

using Holefeeder.Application.Context;
using Holefeeder.Infrastructure.Extensions;
using Holefeeder.Infrastructure.SeedWork;

using Microsoft.EntityFrameworkCore;

using Npgsql;

using Respawn;

namespace Holefeeder.FunctionalTests.Drivers;

public sealed class BudgetingDatabaseDriver : DatabaseDriver<BudgetingContext>
{
    private const string Schema = "budgeting_tests";

    protected override IDatabaseServer DatabaseServer { get; init; }

    protected override RespawnerOptions Options { get; } = new()
    {
        DbAdapter = DbAdapter.Postgres,
        // SchemasToInclude = [Schema],
        TablesToInclude =
            ["accounts", "cashflows", "categories", "store_items", "transactions"],
        TablesToIgnore = ["schema_versions"],
        WithReseed = true
    };

    public BudgetingDatabaseDriver()
    {
        var connectionString = Environment.GetEnvironmentVariable("LOCAL_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            DatabaseServer = PostgreDatabaseServer.CreateServer(Schema);
        }
        else
        {
            DatabaseServer = LocalDatabaseServer.CreateServer(connectionString, ConnectionFactory);
        }
    }

    private static NpgsqlConnection ConnectionFactory(string connectionString) => new(connectionString);

    public override async Task InitializeAsync()
    {
        await DatabaseServer.StartAsync();

        await using (var connection = new NpgsqlConnection(DatabaseServer.ConnectionString))
        {
            await connection.OpenAsync();
            await using (var command = new NpgsqlCommand($"CREATE SCHEMA IF NOT EXISTS {Schema}", connection))
            {
                await command.ExecuteNonQueryAsync();
            }

            // Set the default schema
            await using (var setSchemaCommand = new NpgsqlCommand($"SET search_path TO {Schema}", connection))
            {
                await setSchemaCommand.ExecuteNonQueryAsync();
            }
        }
        await base.InitializeAsync();
    }

    public override BudgetingContext CreateDbContext()
    {
        var context = new BudgetingContext(GetDbContextOptions());
        return context;
    }

    public override DbContextOptions<BudgetingContext> GetDbContextOptions()
    {
        var optionsBuilder = new DbContextOptionsBuilder<BudgetingContext>();

        optionsBuilder.UseNpgsql(ConnectionString);

        return optionsBuilder.Options;
    }

    protected override async Task InitializeDatabaseAsync()
    {
        await using var dbContext = new BudgetingContext(GetDbContextOptions());

        dbContext.Database.MigrateDb(new BudgetingConnectionStringBuilder { ConnectionString = ConnectionString });
    }
}
