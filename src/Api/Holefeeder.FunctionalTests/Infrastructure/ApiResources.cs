namespace Holefeeder.FunctionalTests.Infrastructure;

public enum ApiResources
{
    [ResourceRoute("api/v2/accounts")]
    GetAccounts,
    [ResourceRoute("api/v2/accounts/{0}")]
    GetAccount,
    [ResourceRoute("api/v2/store-items")]
    GetStoreItems,
    [ResourceRoute("api/v2/store-items/{0}")]
    GetStoreItem,
    [ResourceRoute("api/v2/store-items/create-store-item")]
    CreateStoreItem,
    [ResourceRoute("api/v2/store-items/modify-store-item")]
    ModifyStoreItem,
}
