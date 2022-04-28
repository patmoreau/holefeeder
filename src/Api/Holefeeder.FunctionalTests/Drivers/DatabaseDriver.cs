using System.Data.Common;

using Holefeeder.Infrastructure.Extensions;
using Holefeeder.Infrastructure.SeedWork;

using Respawn;

namespace Holefeeder.FunctionalTests.Drivers;

public abstract class DatabaseDriver
{
    protected abstract MySqlDbContext DbContext { get; }

    protected abstract Checkpoint Checkpoint { get; }

    public async Task ResetState()
    {
        await Checkpoint.Reset((DbConnection)DbContext.Connection);
    }

    public async Task Save<T>(T entity) where T : class => await DbContext.Connection.InsertAsync(entity);
}
