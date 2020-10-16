using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Models;

namespace DrifterApps.Holefeeder.Application.Contracts
{
    public interface IUserQueriesRepository
    {
        Task<UserViewModel> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}
