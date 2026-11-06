using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.StoreItems.Core.SearchStoreItems;

internal interface IStoreItems
{
    Task<Result<StoreItemsPage>> SearchAsync(SearchStoreItemsRequest request, CancellationToken cancellationToken);
}
