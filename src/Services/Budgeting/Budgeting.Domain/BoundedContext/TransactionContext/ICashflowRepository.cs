using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext
{
    public interface ICashflowRepository : IRepository<Cashflow>
    {
        Task<Cashflow> FindByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);
        Task SaveAsync(Cashflow cashflow, CancellationToken cancellationToken);
    }
}
