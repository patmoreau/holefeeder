using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.StoreItems.Core.SearchStoreItems;

namespace Holefeeder.Service.StoreItems.Core;

internal interface IStoreItems
{
    Task<Result<StoreItemsPage>> SearchAsync(SearchStoreItemsRequest request, CancellationToken cancellationToken);
}
