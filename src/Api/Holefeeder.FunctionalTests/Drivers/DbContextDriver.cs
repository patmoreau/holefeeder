using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
        List<EntityEntry> entitiesList = DbContext.ChangeTracker.Entries().ToList();
        foreach (EntityEntry entity in entitiesList)
        {
            entity.Reload();
        }
    }
}
