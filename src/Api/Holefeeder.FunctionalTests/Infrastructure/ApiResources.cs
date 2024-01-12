using DrifterApps.Seeds.Testing.Infrastructure;

namespace Holefeeder.FunctionalTests.Infrastructure;

public static class ApiResources
{
    public static readonly ApiResource GetAccounts = ApiResource.DefineApi("api/v2/accounts", HttpMethod.Get);
    public static readonly ApiResource GetAccount = ApiResource.DefineApi("api/v2/accounts/{0}", HttpMethod.Get);
    public static readonly ApiResource CloseAccount = ApiResource.DefineApi("api/v2/accounts/close-account", HttpMethod.Post);
    public static readonly ApiResource ModifyAccount = ApiResource.DefineApi("api/v2/accounts/modify-account", HttpMethod.Post);
    public static readonly ApiResource OpenAccount = ApiResource.DefineApi("api/v2/accounts/open-account", HttpMethod.Post);
    public static readonly ApiResource FavoriteAccount = ApiResource.DefineApi("api/v2/accounts/favorite-account", HttpMethod.Post);

    public static readonly ApiResource GetCashflows = ApiResource.DefineApi("api/v2/cashflows", HttpMethod.Get);
    public static readonly ApiResource GetCashflow = ApiResource.DefineApi("api/v2/cashflows/{0}", HttpMethod.Get);
    public static readonly ApiResource ModifyCashflow = ApiResource.DefineApi("api/v2/cashflows/modify", HttpMethod.Post);
    public static readonly ApiResource CancelCashflow = ApiResource.DefineApi("api/v2/cashflows/cancel", HttpMethod.Post);
    public static readonly ApiResource GetUpcoming = ApiResource.DefineApi("api/v2/cashflows/get-upcoming?from={0}&to={1}", HttpMethod.Get);

    public static readonly ApiResource GetCategories = ApiResource.DefineApi("api/v2/categories", HttpMethod.Get);

    public static readonly ApiResource GetAccountTypes = ApiResource.DefineOpenApi("api/v2/enumerations/get-account-types", HttpMethod.Get);
    public static readonly ApiResource GetCategoryTypes = ApiResource.DefineOpenApi("api/v2/enumerations/get-category-types", HttpMethod.Get);
    public static readonly ApiResource GetDateIntervalTypes = ApiResource.DefineOpenApi("api/v2/enumerations/get-date-interval-types", HttpMethod.Get);

    public static readonly ApiResource ExportData = ApiResource.DefineApi("api/v2/my-data/export-data", HttpMethod.Get);
    public static readonly ApiResource ImportData = ApiResource.DefineApi("api/v2/my-data/import-data", HttpMethod.Post);
    public static readonly ApiResource ImportDataStatus = ApiResource.DefineApi("api/v2/my-data/import-status/{0}", HttpMethod.Get);

    public static readonly ApiResource GetStoreItems = ApiResource.DefineApi("api/v2/store-items", HttpMethod.Get);
    public static readonly ApiResource GetStoreItem = ApiResource.DefineApi("api/v2/store-items/{0}", HttpMethod.Get);
    public static readonly ApiResource CreateStoreItem = ApiResource.DefineApi("api/v2/store-items/create-store-item", HttpMethod.Post);
    public static readonly ApiResource ModifyStoreItem = ApiResource.DefineApi("api/v2/store-items/modify-store-item", HttpMethod.Post);

    public static readonly ApiResource MakePurchase = ApiResource.DefineApi("api/v2/transactions/make-purchase", HttpMethod.Post);
    public static readonly ApiResource PayCashflow = ApiResource.DefineApi("api/v2/transactions/pay-cashflow", HttpMethod.Post);
    public static readonly ApiResource Transfer = ApiResource.DefineApi("api/v2/transactions/transfer", HttpMethod.Post);
    public static readonly ApiResource ModifyTransaction = ApiResource.DefineApi("api/v2/transactions/modify", HttpMethod.Post);
    public static readonly ApiResource DeleteTransaction = ApiResource.DefineApi("api/v2/transactions/{0}", HttpMethod.Delete);
    public static readonly ApiResource GetTransaction = ApiResource.DefineApi("api/v2/transactions/{0}", HttpMethod.Get);
    public static readonly ApiResource GetTransactions = ApiResource.DefineApi("api/v2/transactions", HttpMethod.Get);

    public static readonly ApiResource GetForAllCategories = ApiResource.DefineApi("api/v2/categories/statistics", HttpMethod.Get);
    public static readonly ApiResource GetSummary = ApiResource.DefineApi("api/v2/summary/statistics?from={0}&to={1}", HttpMethod.Get);

    public static readonly ApiResource GetTagsWithCount = ApiResource.DefineApi("api/v2/tags", HttpMethod.Get);
}
