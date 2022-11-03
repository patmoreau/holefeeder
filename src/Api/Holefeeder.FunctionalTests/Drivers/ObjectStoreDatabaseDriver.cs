using System.Data.Common;

using Holefeeder.Application.Context;

using Microsoft.Extensions.DependencyInjection;

using Respawn;
using Respawn.Graph;

namespace Holefeeder.FunctionalTests.Drivers;

public sealed class ObjectStoreDatabaseDriver : DbContextDriver, IDisposable
{
    private IServiceScope _scope;
    public ObjectStoreDatabaseDriver(ApiApplicationDriver apiApplicationDriver)
    {
        if (apiApplicationDriver == null)
        {
            throw new ArgumentNullException(nameof(apiApplicationDriver));
        }
        _scope = apiApplicationDriver.Server.Services
            .GetService<IServiceScopeFactory>()!.CreateScope();

        var context = _scope.ServiceProvider.GetRequiredService<StoreItemContext>();

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
        _scope.Dispose();
    }
}
