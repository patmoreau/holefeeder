using System.Data.Common;

using Holefeeder.Application.Context;

using Respawn;
using Respawn.Graph;

namespace Holefeeder.FunctionalTests.Drivers;

public sealed class BudgetingDatabaseDriver : DbContextDriver
{
    public BudgetingDatabaseDriver(BudgetingContext context)
    {
        DbContext = context;
    }

    protected override BudgetingContext DbContext { get; }

    protected override async Task<Respawner> CreateStateAsync(DbConnection connection)
    {
        return await Respawner.CreateAsync(connection,
            new RespawnerOptions
            {
                SchemasToInclude = new[] {"budgeting_functional_tests"},
                DbAdapter = DbAdapter.MySql,
                TablesToInclude = new Table[] {"accounts", "cashflows", "categories", "store_items", "transactions"},
                TablesToIgnore = new Table[] {"schema_versions"}
            });
    }
}
