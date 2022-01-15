using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

namespace DrifterApps.Holefeeder.Budgeting.Application.Transactions;

public interface ITransactionQueriesRepository
{
    Task<(int Total, IEnumerable<TransactionInfoViewModel> Items)> FindAsync(Guid userId, QueryParams queryParams,
        CancellationToken cancellationToken);

    Task<TransactionInfoViewModel?> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken);
}
