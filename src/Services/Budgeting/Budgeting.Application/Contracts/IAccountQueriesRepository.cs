using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Application.Queries;

namespace DrifterApps.Holefeeder.Budgeting.Application.Contracts
{
    public interface IAccountQueriesRepository
    {
        Task<IEnumerable<AccountViewModel>> GetAccountsAsync(Guid userId, QueryParams queryParams,
            CancellationToken cancellationToken = default);
    }
}
