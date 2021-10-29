using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

namespace DrifterApps.Holefeeder.Budgeting.Application.Transactions
{
    public interface ITransactionQueriesRepository
    {
        Task<QueryResult<TransactionViewModel>> FindAsync(Guid userId, QueryParams queryParams,
            CancellationToken cancellationToken);

        Task<TransactionViewModel> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken);
    }
}
