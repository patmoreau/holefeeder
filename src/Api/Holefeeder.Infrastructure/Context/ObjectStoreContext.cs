using Holefeeder.Infrastructure.SeedWork;

namespace Holefeeder.Infrastructure.Context;

public class ObjectStoreContext : MySqlDbContext, IObjectStoreContext
{
    public ObjectStoreContext(ObjectStoreDatabaseSettings settings) : base(settings)
    {
    }
}
