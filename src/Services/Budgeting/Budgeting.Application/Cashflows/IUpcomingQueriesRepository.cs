using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Models;

namespace DrifterApps.Holefeeder.Budgeting.Application.Cashflows
{
    public interface IUpcomingQueriesRepository
    {
        Task<IEnumerable<UpcomingViewModel>> GetUpcomingAsync(Guid userId, DateTime startDate, DateTime endDate,
            CancellationToken cancellationToken = default);
    }
}
