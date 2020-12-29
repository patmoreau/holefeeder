using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Framework.SeedWork;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories
{
    public abstract class RepositoryRoot
    {
        protected IMongoDbContext DbContext { get; }
        
        protected RepositoryRoot(IMongoDbContext context)
        {
            DbContext = context.ThrowIfNull(nameof(context));
        }
    }
}
