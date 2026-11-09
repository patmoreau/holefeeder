using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.StoreItems.Core.GetStoreItem;

internal sealed class GetStoreItemUseCase(IStoreItems storeItems)
{
    public Task<Result<StoreItem>> HandleAsync(GetStoreItemRequest request, CancellationToken cancellationToken) =>
        storeItems.GetAsync(request, cancellationToken);
}
