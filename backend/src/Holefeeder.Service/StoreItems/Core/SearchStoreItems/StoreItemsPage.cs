using Holefeeder.Service.Shared.Core.Search;

namespace Holefeeder.Service.StoreItems.Core.SearchStoreItems;

internal sealed record StoreItemsPage(int Total, IReadOnlyList<StoreItem> Items) : PagedData<StoreItem>(Total, Items);
