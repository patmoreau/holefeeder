using System.Collections.Generic;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;

namespace DrifterApps.Holefeeder.Business
{
    public interface ITransactionsService : IBaseOwnedService<TransactionEntity>
    {
        Task<long> CountAsync(string userId, QueryParams query);
        Task<IEnumerable<TransactionDetailEntity>> FindWithDetailsAsync(string userId, QueryParams query);
    }
}