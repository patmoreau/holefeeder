using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Business.Entities;

namespace DrifterApps.Holefeeder.ResourcesAccess
{
    public interface IUsersRepository : IBaseRepository<UserEntity>
    {
        Task<UserEntity> FindByEmailAsync(string emailAddress, CancellationToken cancellationToken = default);
    }
}