using Framework.Dapper.SeedWork;

namespace DrifterApps.Holefeeder.ObjectStore.Infrastructure.Context;

public class ObjectStoreContext : MySqlDbContext, IObjectStoreContext
{
    public ObjectStoreContext(ObjectStoreDatabaseSettings settings) : base(settings)
    {
    }
}
