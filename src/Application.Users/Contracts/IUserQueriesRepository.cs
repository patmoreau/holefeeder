using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Users.Models;

namespace DrifterApps.Holefeeder.Application.Users.Contracts
{
    public interface IUserQueriesRepository
    {
        Task<UserViewModel> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}
