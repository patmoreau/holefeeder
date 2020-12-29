using System;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext
{
    public interface IStoreRepository : IRepository<StoreItem>
    {
        Task<StoreItem> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
        Task<StoreItem> FindByCodeAsync(Guid userId, string code, CancellationToken cancellationToken = default);
        Task SaveAsync(StoreItem entity, CancellationToken cancellationToken = default);
    }
}
