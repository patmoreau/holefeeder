using System.Data.Common;

using Holefeeder.Infrastructure.Extensions;
using Holefeeder.Infrastructure.SeedWork;

using Respawn;

namespace Holefeeder.FunctionalTests.Drivers;

public abstract class DatabaseDriver
{
    protected abstract MySqlDbContext DbContext { get; }

    protected abstract Checkpoint Checkpoint { get; }

    public async Task ResetStateAsync()
    {
        await Checkpoint.Reset((DbConnection)DbContext.Connection);
    }

    public async Task SaveAsync<T>(T entity) where T : class => await DbContext.Connection.InsertAsync(entity);

    public async Task<T?> FindByIdAsync<T>(Guid id, Guid userId) where T : class =>
        await DbContext.Connection.FindByIdAsync<T>(new {Id = id, UserId = userId});
}
