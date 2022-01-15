using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

namespace DrifterApps.Holefeeder.Budgeting.Application.Cashflows;

public interface ICashflowQueriesRepository
{
    Task<(int Total, IEnumerable<CashflowInfoViewModel> Items)> FindAsync(Guid userId, QueryParams queryParams,
        CancellationToken cancellationToken);

    Task<CashflowInfoViewModel?> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken);
}
