using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Business.Entities;

namespace DrifterApps.Holefeeder.Business
{
    public interface ICashflowsService : IBaseOwnedService<CashflowEntity>
    {
        Task<IEnumerable<UpcomingEntity>> GetUpcomingAsync(Guid userId, (DateTime From, DateTime To) interval, CancellationToken cancellationToken = default);
    }
}
