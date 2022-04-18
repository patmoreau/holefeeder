using Holefeeder.Domain.SeedWork;

namespace Holefeeder.Domain.Features.Accounts;

public interface IAccountRepository : IRepository<Account>
{
    Task<Account?> FindByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<Account?> FindByNameAsync(string name, Guid userId, CancellationToken cancellationToken);
    Task SaveAsync(Account account, CancellationToken cancellationToken);
}
