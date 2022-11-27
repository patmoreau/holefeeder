using System.Data.Common;

using Holefeeder.Application.Context;

using Respawn;
using Respawn.Graph;

namespace Holefeeder.FunctionalTests.Drivers;

public sealed class ObjectStoreDatabaseDriver : DbContextDriver, IDisposable
{
    public ObjectStoreDatabaseDriver(StoreItemContext context)
    {
        DbContext = context;
    }

    protected override StoreItemContext DbContext { get; }

    protected override async Task<Respawner> CreateStateAsync(DbConnection connection)
    {
        return await Respawner.CreateAsync(connection,
            new RespawnerOptions
            {
                SchemasToInclude = new[] {"object_store_functional_tests"},
                DbAdapter = DbAdapter.MySql,
                TablesToInclude = new Table[] {"store_items"},
                TablesToIgnore = new Table[] {"schema_versions"}
            });
    }

    public void Dispose()
    {
        DbContext.Dispose();
    }
}
