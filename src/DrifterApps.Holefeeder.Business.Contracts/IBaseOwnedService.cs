using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;

namespace DrifterApps.Holefeeder.Business
{
    public interface IBaseOwnedService<TEntity> : IBaseService<TEntity> where TEntity : IIdentityEntity, IOwnedEntity<TEntity>
    {
        Task<bool> IsOwnerAsync(string userId, string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindAsync(string userId, QueryParams queryParams, CancellationToken cancellationToken = default);
        Task<TEntity> CreateAsync(string userId, TEntity entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(string userId, string id, TEntity entity, CancellationToken cancellationToken = default);
    }
}
