using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.StoreItems.Core.SearchStoreItems;

internal sealed class SearchStoreItemsUseCase(IStoreItems storeItems)
{
    public Task<Result<StoreItemsPage>> HandleAsync(SearchStoreItemsRequest request, CancellationToken cancellationToken) =>
        storeItems.SearchAsync(request, cancellationToken);
}
