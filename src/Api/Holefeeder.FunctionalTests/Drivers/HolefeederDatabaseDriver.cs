using Holefeeder.Infrastructure.Context;
using Holefeeder.Infrastructure.SeedWork;

using Microsoft.Extensions.DependencyInjection;

using Respawn;
using Respawn.Graph;

namespace Holefeeder.FunctionalTests.Drivers;

public class HolefeederDatabaseDriver : DatabaseDriver
{
    protected override MySqlDbContext DbContext { get; }

    protected override Checkpoint Checkpoint { get; } = new()
    {
        SchemasToInclude = new[] {"budgeting_functional_tests"},
        DbAdapter = DbAdapter.MySql,
        TablesToInclude = new Table[] {"accounts", "cashflows", "categories", "transactions"},
        TablesToIgnore = new Table[] {"schema_versions"}
    };

    public HolefeederDatabaseDriver(ApiApplicationDriver apiApplicationDriver)
    {
        var settings = apiApplicationDriver.Services.GetRequiredService<HolefeederDatabaseSettings>();
        var context = new HolefeederContext(settings);

        DbContext = context;
    }
}
