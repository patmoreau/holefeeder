using System.Collections.Generic;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;

namespace DrifterApps.Holefeeder.ResourcesAccess
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<long> CountAsync(QueryParams query);
        Task<IEnumerable<T>> FindAsync(QueryParams queryParams);
        Task<T> FindByIdAsync(string id);
        Task<T> CreateAsync(T entity);
        Task UpdateAsync(string id, T entity);
        Task RemoveAsync(string id);
    }
}