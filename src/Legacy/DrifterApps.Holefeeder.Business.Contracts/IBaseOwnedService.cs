using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;

namespace DrifterApps.Holefeeder.Business
{
    public interface IBaseOwnedService<TEntity> : IBaseService<TEntity> where TEntity : IIdentityEntity, IOwnedEntity<TEntity>
    {
        Task<bool> IsOwnerAsync(Guid userId, string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindAsync(Guid userId, QueryParams queryParams, CancellationToken cancellationToken = default);
        Task<TEntity> CreateAsync(Guid userId, TEntity entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid userId, string id, TEntity entity, CancellationToken cancellationToken = default);
    }
}
