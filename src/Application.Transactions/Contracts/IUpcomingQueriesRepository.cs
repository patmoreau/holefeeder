using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Transactions.Models;

namespace DrifterApps.Holefeeder.Application.Transactions.Contracts
{
    public interface IUpcomingQueriesRepository
    {
        Task<IEnumerable<UpcomingViewModel>> GetUpcomingAsync(Guid userId, DateTime startDate, DateTime endDate,
            CancellationToken cancellationToken = default);
    }
}
