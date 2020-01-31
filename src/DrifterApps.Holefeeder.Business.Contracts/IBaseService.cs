using System.Collections.Generic;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;

namespace DrifterApps.Holefeeder.Business
{
    public interface IBaseService<TEntity> where TEntity : IIdentityEntity
    {
        Task DeleteAsync(string id);
        Task<TEntity> FindByIdAsync(string id);
        Task<IEnumerable<TEntity>> FindAsync(QueryParams queryParams);
        Task<TEntity> CreateAsync(TEntity entity);
        Task UpdateAsync(string id, TEntity entity);
    }
}