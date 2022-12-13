namespace Holefeeder.FunctionalTests.Infrastructure;

public enum ApiResources
{
    [ResourceRoute("api/v2/categories")] GetCategories,
    [ResourceRoute("api/v2/accounts")] GetAccounts,
    [ResourceRoute("api/v2/accounts/{0}")] GetAccount,

    [ResourceRoute("api/v2/accounts/close-account")]
    CloseAccount,

    [ResourceRoute("api/v2/accounts/modify-account")]
    ModifyAccount,

    [ResourceRoute("api/v2/accounts/open-account")]
    OpenAccount,

    [ResourceRoute("api/v2/accounts/favorite-account")]
    FavoriteAccount,
    [ResourceRoute("api/v2/store-items")] GetStoreItems,

    [ResourceRoute("api/v2/store-items/{0}")]
    GetStoreItem,

    [ResourceRoute("api/v2/store-items/create-store-item")]
    CreateStoreItem,

    [ResourceRoute("api/v2/store-items/modify-store-item")]
    ModifyStoreItem,

    [ResourceRoute("api/v2/enumerations/get-account-types")]
    GetAccountTypes,

    [ResourceRoute("api/v2/enumerations/get-category-types")]
    GetCategoryTypes,

    [ResourceRoute("api/v2/enumerations/get-date-interval-types")]
    GetDateIntervalTypes,

    [ResourceRoute("api/v2/transactions/make-purchase")]
    MakePurchase,

    [ResourceRoute("api/v2/cashflows/modify")]
    ModifyCashflow,

    [ResourceRoute("api/v2/cashflows/cancel")]
    CancelCashflow,

    [ResourceRoute("api/v2/my-data/export-data")]
    ExportData,

    [ResourceRoute("api/v2/my-data/import-data")]
    ImportData,

    [ResourceRoute("api/v2/my-data/import-status/{0}")]
    ImportDataStatus,
}
