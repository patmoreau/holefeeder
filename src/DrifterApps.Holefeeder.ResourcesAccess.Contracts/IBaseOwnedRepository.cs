using System.Collections.Generic;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;

namespace DrifterApps.Holefeeder.ResourcesAccess
{
    public interface IBaseOwnedRepository<T> : IBaseRepository<T> where T : BaseEntity, IOwnedEntity
    {
        Task<bool> IsOwnerAsync(string userId, string id);
        Task<long> CountAsync(string userId, QueryParams query);
        Task<IEnumerable<T>> FindAsync(string userId, QueryParams queryParams);
    }
}
