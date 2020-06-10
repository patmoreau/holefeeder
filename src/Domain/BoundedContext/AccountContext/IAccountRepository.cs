using System;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Domain.SeedWork;

namespace DrifterApps.Holefeeder.Domain.BoundedContext.AccountContext
{
    public interface IAccountRepository : IRepository<AccountContext.Account>
    {
        Task<AccountContext.Account> FindByIdAsync(Guid id, CancellationToken cancellationToken);
        Task CreateAsync(AccountContext.Account account, CancellationToken cancellationToken);
        Task UpdateAsync(AccountContext.Account account, CancellationToken cancellationToken);
    }
}
