using System;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Budgeting.Domain.SeedWork;

namespace DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<Transaction> FindByIdAsync(Guid id, CancellationToken cancellationToken);
        Task CreateAsync(Transaction transaction, CancellationToken cancellationToken);
        Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken);
        Task<bool> IsAccountActive(Guid id, CancellationToken cancellationToken);
    }
}
