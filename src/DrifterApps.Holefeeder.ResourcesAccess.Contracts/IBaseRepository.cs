using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;

namespace DrifterApps.Holefeeder.ResourcesAccess
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<int> CountAsync(QueryParams query, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> FindAsync(QueryParams queryParams, CancellationToken cancellationToken = default);
        Task<T> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(string id, T entity, CancellationToken cancellationToken = default);
        Task RemoveAsync(string id, CancellationToken cancellationToken = default);
    }
}