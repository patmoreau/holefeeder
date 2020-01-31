using System.Collections.Generic;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;

namespace DrifterApps.Holefeeder.Business
{
    public interface IBaseOwnedService<TEntity> : IBaseService<TEntity> where TEntity : IIdentityEntity, IOwnedEntity<TEntity>
    {
        Task<bool> IsOwnerAsync(string userId, string id);
        Task<IEnumerable<TEntity>> FindAsync(string userId, QueryParams queryParams);
        Task<TEntity> CreateAsync(string userId, TEntity entity);
    }
}