using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Business.Entities;

namespace DrifterApps.Holefeeder.Business
{
    public interface IUsersService : IBaseService<UserEntity>
    {
        Task<UserEntity> FindByEmailAsync(string emailAddress, CancellationToken cancellationToken = default);
    }
}