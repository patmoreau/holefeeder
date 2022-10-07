using System.Data.Common;

using Holefeeder.Infrastructure.Context;
using Holefeeder.Infrastructure.SeedWork;

using Microsoft.Extensions.DependencyInjection;

using Respawn;
using Respawn.Graph;

namespace Holefeeder.FunctionalTests.Drivers;

public class ObjectStoreDatabaseDriver : DatabaseDriver
{
    protected override MySqlDbContext DbContext { get; }

    public ObjectStoreDatabaseDriver(ApiApplicationDriver apiApplicationDriver)
    {
        if (apiApplicationDriver == null)
        {
            throw new ArgumentNullException(nameof(apiApplicationDriver));
        }

        var settings = apiApplicationDriver.Services.GetRequiredService<ObjectStoreDatabaseSettings>();
        var context = new ObjectStoreContext(settings);

        DbContext = context;
    }

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
}
