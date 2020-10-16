using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Models;

namespace DrifterApps.Holefeeder.Application.Contracts
{
    public interface IUpcomingQueriesRepository
    {
        Task<IEnumerable<UpcomingViewModel>> GetUpcomingAsync(Guid userId, DateTime from, DateTime to,
            CancellationToken cancellationToken = default);
    }
}
