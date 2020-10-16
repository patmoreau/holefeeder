using System;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Domain.SeedWork;

namespace DrifterApps.Holefeeder.Domain.AggregatesModel.AccountAggregate
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account> FindByIdAsync(Guid id, CancellationToken cancellationToken);
        Task CreateAsync(Account account, CancellationToken cancellationToken);
        Task UpdateAsync(Account account, CancellationToken cancellationToken);
    }
}
