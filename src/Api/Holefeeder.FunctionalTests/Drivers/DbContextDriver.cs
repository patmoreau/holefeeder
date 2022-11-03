using System.Data.Common;

using Microsoft.EntityFrameworkCore;

using Respawn;

namespace Holefeeder.FunctionalTests.Drivers;

public abstract class DbContextDriver
{
    protected abstract DbContext DbContext { get; }

    private Respawner? Checkpoint { get; set; }

    protected abstract Task<Respawner> CreateStateAsync(DbConnection connection);

    public async Task ResetStateAsync()
    {
        var connection = DbContext.Database.GetDbConnection();
        await connection.OpenAsync();
        Checkpoint ??= await CreateStateAsync(connection);
        await Checkpoint.ResetAsync(connection);
    }

    public async Task SaveAsync<T>(T entity) where T : class
    {
        DbContext.Add(entity);
        await DbContext.SaveChangesAsync();
    }

    public async Task<T?> FindByIdAsync<T>(Guid id, Guid userId) where T : class
    {
        return await DbContext.FindAsync<T>(id, userId);
    }
}
