using System.Data.Common;

using Holefeeder.Infrastructure.Context;
using Holefeeder.Infrastructure.SeedWork;

using Respawn;
using Respawn.Graph;

namespace Holefeeder.FunctionalTests.Drivers;

public class HolefeederDatabaseDriver : DatabaseDriver
{
    protected override MySqlDbContext DbContext { get; }

    public HolefeederDatabaseDriver(HolefeederContext context)
    {
        DbContext = context;
    }

    protected override async Task<Respawner> CreateStateAsync(DbConnection connection)
    {
        return await Respawner.CreateAsync(connection,
            new RespawnerOptions
            {
                SchemasToInclude = new[] {"budgeting_functional_tests"},
                DbAdapter = DbAdapter.MySql,
                TablesToInclude = new Table[] {"accounts", "cashflows", "categories", "transactions"},
                TablesToIgnore = new Table[] {"schema_versions"}
            });
    }
}
