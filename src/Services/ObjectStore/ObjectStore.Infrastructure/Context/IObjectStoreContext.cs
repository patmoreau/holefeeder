using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using Framework.Dapper.SeedWork;

namespace DrifterApps.Holefeeder.ObjectStore.Infrastructure.Context
{
    public interface IObjectStoreContext : IDbContext, IUnitOfWork
    {
    }
}
