using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;

namespace DrifterApps.Holefeeder.Business
{
    public interface IBaseService<TEntity> where TEntity : IIdentityEntity
    {
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
        Task<TEntity> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindAsync(QueryParams queryParams, CancellationToken cancellationToken = default);
        Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(string id, TEntity entity, CancellationToken cancellationToken = default);
    }
}