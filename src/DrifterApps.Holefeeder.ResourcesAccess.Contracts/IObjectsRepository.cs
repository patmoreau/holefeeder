using System;
using DrifterApps.Holefeeder.Business.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace DrifterApps.Holefeeder.ResourcesAccess
{
    public interface IObjectsRepository : IBaseOwnedRepository<ObjectDataEntity>
    {
        Task<ObjectDataEntity> FindByCodeAsync(Guid userId, string code, CancellationToken cancellationToken = default);
    }
}
