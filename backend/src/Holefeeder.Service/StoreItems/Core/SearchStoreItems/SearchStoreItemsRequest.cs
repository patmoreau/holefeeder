using Holefeeder.Service.Shared.Core.Search;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.StoreItems.Core.SearchStoreItems;

internal record SearchStoreItemsRequest(User User, Offset Offset, Limit Limit, SearchStoreItemsRequestFilterBy FilterBy, SearchStoreItemsRequestSortBy SortBy);
