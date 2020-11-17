using System;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Infrastructure.Database.Context;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DrifterApps.Holefeeder.Infrastructure.Database.Repositories
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
