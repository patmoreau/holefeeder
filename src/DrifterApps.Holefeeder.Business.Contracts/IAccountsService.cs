using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;

namespace DrifterApps.Holefeeder.Business
{
    public interface IAccountsService : IBaseOwnedService<AccountEntity>
    {
        Task<IEnumerable<AccountDetailEntity>> FindWithDetailsAsync(string userId, QueryParams queryParams, CancellationToken cancellationToken = default);
    }
}