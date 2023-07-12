using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.Infrastructure.Persistence;
using Holefeeder.Application.Context;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Respawn;
using Respawn.Graph;

namespace Holefeeder.FunctionalTests.Drivers;

public sealed class BudgetingDatabaseDriver : DatabaseDriver<BudgetingContext>
{
    private const string Schema = "budgeting";

    protected sealed override IDatabaseServer DatabaseServer { get; init; }

    protected override RespawnerOptions Options { get; } = new()
    {
        DbAdapter = DbAdapter.MySql,
        SchemasToInclude = new[] { Schema },
        // TablesToInclude =
        //     new Table[] { "accounts", "cashflows", "categories", "store_items", "transactions" },
        TablesToIgnore = new Table[] { "schema_versions" },
        WithReseed = true
    };

    public BudgetingDatabaseDriver()
    {

#if DEBUG
        DatabaseServer = MariaDatabaseServer.CreateServer(Schema, 62881);
#else
        DatabaseServer = MariaDatabaseServer.CreateServer(Schema);
#endif
    }

    public override async Task InitializeAsync()
    {
        await DatabaseServer.StartAsync();

        await base.InitializeAsync();
    }

    protected override async Task InitializeDatabaseAsync()
    {
        DbContext = new BudgetingContext(GetDbContextOptions());

        await DbContext.Database.EnsureCreatedAsync();
        // await DbContext.Database.MigrateAsync();
    }

    public override BudgetingContext DbContext { get; protected set; } = default!;

    public override DbContextOptions<BudgetingContext> GetDbContextOptions()
    {
        string connectionString = DatabaseServer.ConnectionString;
        var optionsBuilder = new DbContextOptionsBuilder<BudgetingContext>();

        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return optionsBuilder.Options;
    }
}
