using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

namespace DrifterApps.Holefeeder.Budgeting.Application.Accounts;

public interface IAccountQueriesRepository
{
    Task<(int Total, IEnumerable<AccountViewModel> Items)> FindAsync(Guid userId, QueryParams queryParams,
        CancellationToken cancellationToken);

    Task<AccountViewModel?> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken);

    Task<bool> IsAccountActive(Guid id, Guid userId, CancellationToken cancellationToken);
}
