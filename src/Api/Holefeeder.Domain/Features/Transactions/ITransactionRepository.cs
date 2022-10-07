using Holefeeder.Domain.SeedWork;

namespace Holefeeder.Domain.Features.Transactions;

public interface ITransactionRepository : IRepository
{
    Task<Transaction?> FindByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task SaveAsync(Transaction transaction, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<bool> AccountExists(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<bool> CategoryExists(Guid id, Guid userId, CancellationToken cancellationToken);
}
