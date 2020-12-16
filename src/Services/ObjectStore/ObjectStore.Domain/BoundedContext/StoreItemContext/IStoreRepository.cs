using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext
{
    public interface IStoreRepository : IRepository<StoreItem>
    {
        Task CreateAsync(StoreItem entity, CancellationToken cancellationToken);
    }
}
