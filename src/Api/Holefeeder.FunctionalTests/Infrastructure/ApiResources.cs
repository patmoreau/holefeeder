namespace Holefeeder.FunctionalTests.Infrastructure;

public enum ApiResources
{
    [ResourceRoute("api/v2/budgeting")]
    Budgeting,
    [ResourceRoute("api/v2/store-items")]
    StoreItems,
    [ResourceRoute("api/v2/store-items?offset={0}&limit={1}&sort={2}&filter={3}")]
    StoreItemsQuery,
    [ResourceRoute("api/v2/store-items/{0}")]
    StoreItem,
    [ResourceRoute("api/v2/store-items/create-store-item")]
    CreateStoreItem,
    [ResourceRoute("api/v2/store-items/modify-store-item")]
    ModifyStoreItem,
}
