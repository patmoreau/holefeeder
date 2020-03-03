using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;

namespace DrifterApps.Holefeeder.Business
{
    public interface ITransactionsService : IBaseOwnedService<TransactionEntity>
    {
        Task<int> CountAsync(string userId, QueryParams query, CancellationToken cancellationToken = default);
        Task<IEnumerable<TransactionDetailEntity>> FindWithDetailsAsync(string userId, QueryParams query, CancellationToken cancellationToken = default);
    }
}