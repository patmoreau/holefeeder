using Microsoft.EntityFrameworkCore;

namespace Holefeeder.FunctionalTests.Drivers;

public abstract class DbContextDriver
{
    protected abstract DbContext DbContext { get; }

    public async Task SaveAsync<T>(T entity) where T : class
    {
        DbContext.Add(entity);
        await DbContext.SaveChangesAsync();
    }

    public async Task<T?> FindByIdAsync<T>(Guid id) where T : class
    {
        RefreshAll();
        return await DbContext.FindAsync<T>(id);
    }

    private void RefreshAll()
    {
        var entitiesList = DbContext.ChangeTracker.Entries().ToList();
        foreach (var entity in entitiesList)
        {
            entity.Reload();
        }
    }
}
