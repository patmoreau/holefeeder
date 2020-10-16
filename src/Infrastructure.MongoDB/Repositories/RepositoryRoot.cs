using System;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Infrastructure.MongoDB.Context;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DrifterApps.Holefeeder.Infrastructure.MongoDB.Repositories
{
    public abstract class RepositoryRoot
    {
        protected IMongoDbContext DbContext { get; }
        
        protected RepositoryRoot(IMongoDbContext context)
        {
            DbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected async Task<string> GetUserMongoId(Guid id, CancellationToken cancellationToken)
        {
            var users = await DbContext.GetUsersAsync(cancellationToken);

            var user = await users.AsQueryable().SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

            return user?.MongoId;
        }
    }
}
