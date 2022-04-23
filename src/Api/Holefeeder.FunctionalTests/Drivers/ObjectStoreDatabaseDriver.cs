using Holefeeder.Infrastructure.Context;
using Holefeeder.Infrastructure.SeedWork;

using Microsoft.Extensions.DependencyInjection;

using Respawn;
using Respawn.Graph;

namespace Holefeeder.FunctionalTests.Drivers;

public class ObjectStoreDatabaseDriver : DatabaseDriver
{
    protected override MySqlDbContext DbContext { get; }

    protected override Checkpoint Checkpoint { get; } = new()
    {
        SchemasToInclude = new[] {"object_store_functional_tests"},
        DbAdapter = DbAdapter.MySql,
        TablesToInclude = new Table[] {"store_items"},
        TablesToIgnore = new Table[] {"schema_versions"}
    };

    public ObjectStoreDatabaseDriver(ApiApplicationDriver apiApplicationDriver)
    {
        var settings = apiApplicationDriver.Services.GetRequiredService<ObjectStoreDatabaseSettings>();
        var context = new ObjectStoreContext(settings);

        DbContext = context;
    }
}
