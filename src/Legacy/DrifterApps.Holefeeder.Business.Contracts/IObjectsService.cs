using System;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Business.Entities;

namespace DrifterApps.Holefeeder.Business
{
    public interface IObjectsService : IBaseOwnedService<ObjectDataEntity>
    {
        Task<ObjectDataEntity> FindByCodeAsync(Guid userId, string code, CancellationToken cancellationToken = default);
    }
}
