using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext
{
    public interface IStoreItemsRepository : IRepository<StoreItem>
    {
        Task<StoreItem> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken);
        Task<StoreItem> FindByCodeAsync(Guid userId, string code, CancellationToken cancellationToken);
        Task SaveAsync(StoreItem entity, CancellationToken cancellationToken);
    }
}
