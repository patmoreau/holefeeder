using System.Threading.Tasks;
using DrifterApps.Holefeeder.Business.Entities;

namespace DrifterApps.Holefeeder.ResourcesAccess
{
    public interface IObjectsRepository : IBaseOwnedRepository<ObjectDataEntity>
    {
        Task<ObjectDataEntity> FindByCodeAsync(string userId, string code);
    }
}