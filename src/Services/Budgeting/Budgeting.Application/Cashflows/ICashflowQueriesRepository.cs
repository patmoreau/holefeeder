using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

namespace DrifterApps.Holefeeder.Budgeting.Application.Cashflows
{
    public interface ICashflowQueriesRepository
    {
        Task<QueryResult<CashflowViewModel>> FindAsync(Guid userId, QueryParams queryParams,
            CancellationToken cancellationToken);

        Task<CashflowViewModel> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken);
    }
}
