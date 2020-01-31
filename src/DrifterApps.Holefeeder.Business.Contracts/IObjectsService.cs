using System.Threading.Tasks;
using DrifterApps.Holefeeder.Business.Entities;

namespace DrifterApps.Holefeeder.Business
{
    public interface IObjectsService : IBaseOwnedService<ObjectDataEntity>
    {
        Task<ObjectDataEntity> FindByCodeAsync(string userId, string code);
    }
}