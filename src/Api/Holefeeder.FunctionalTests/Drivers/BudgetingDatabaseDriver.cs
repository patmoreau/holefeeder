using System.Data.Common;

using Holefeeder.Application.Context;

using Microsoft.Extensions.DependencyInjection;

using Respawn;
using Respawn.Graph;

namespace Holefeeder.FunctionalTests.Drivers;

public sealed class BudgetingDatabaseDriver : DbContextDriver, IDisposable
{
    private readonly IServiceScope _scope;

    public BudgetingDatabaseDriver(ApiApplicationDriver apiApplicationDriver)
    {
        if (apiApplicationDriver == null)
        {
            throw new ArgumentNullException(nameof(apiApplicationDriver));
        }
        _scope = apiApplicationDriver.Server.Services
            .GetService<IServiceScopeFactory>()!.CreateScope();

        var context = _scope.ServiceProvider.GetRequiredService<BudgetingContext>();

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
                TablesToInclude = new Table[] {"accounts", "cashflows", "categories", "transactions"},
                TablesToIgnore = new Table[] {"schema_versions"}
            });
    }

    public void Dispose()
    {
        DbContext.Dispose();
        _scope.Dispose();
    }
}
