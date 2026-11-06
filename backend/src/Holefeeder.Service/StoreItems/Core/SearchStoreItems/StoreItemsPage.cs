namespace Holefeeder.Service.StoreItems.Core.SearchStoreItems;

internal sealed record StoreItemsPage(int Total, IReadOnlyList<StoreItem> Items);
