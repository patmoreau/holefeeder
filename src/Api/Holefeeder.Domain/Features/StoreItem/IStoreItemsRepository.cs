using Holefeeder.Domain.SeedWork;

namespace Holefeeder.Domain.Features.StoreItem;

public interface IStoreItemsRepository : IRepository<StoreItem>
{
    Task<StoreItem?> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken);
    Task<StoreItem?> FindByCodeAsync(Guid userId, string code, CancellationToken cancellationToken);
    Task SaveAsync(StoreItem model, CancellationToken cancellationToken);
}
