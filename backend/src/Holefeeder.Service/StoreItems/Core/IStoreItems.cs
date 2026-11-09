using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.StoreItems.Core.GetStoreItem;
using Holefeeder.Service.StoreItems.Core.SearchStoreItems;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.StoreItems.Core;

internal interface IStoreItems
{
    Task<Result<StoreItemsPage>> SearchAsync(SearchStoreItemsRequest request, CancellationToken cancellationToken);

    Task<Result<StoreItem>> GetAsync(GetStoreItemRequest request, CancellationToken cancellationToken);

    Task<Result<Nothing>> SaveAsync(User user, StoreItem item, CancellationToken cancellationToken);
}
