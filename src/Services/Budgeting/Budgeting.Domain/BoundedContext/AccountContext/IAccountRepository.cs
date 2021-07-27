using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account> FindByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);
        Task<Account> FindByNameAsync(string name, Guid userId, CancellationToken cancellationToken);
        Task SaveAsync(Account account, CancellationToken cancellationToken);
    }
}
