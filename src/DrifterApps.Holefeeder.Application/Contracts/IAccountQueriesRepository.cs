using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Models;
using DrifterApps.Holefeeder.Application.Queries;

namespace DrifterApps.Holefeeder.Application.Contracts
{
    public interface IAccountQueriesRepository
    {
        Task<IEnumerable<AccountViewModel>> GetAccountsAsync(Guid userId, QueryParams queryParams, CancellationToken cancellationToken);
    }
}
