using Holefeeder.Ui.Common.Models;

using Refit;

namespace Holefeeder.Ui.Common.Services;

public interface IHolefeederApiService
{
    [Get("/api/v2/accounts")]
    Task<IApiResponse<IList<Account>>> GetAccounts([Query(CollectionFormat.Multi)] string[] sort, [Query(CollectionFormat.Multi)] string[] filter);

    [Get("/api/v2/enumerations/get-account-types")]
    Task<IApiResponse<IList<string>>> GetAccountTypes();

    [Get("/api/v2/categories")]
    Task<IApiResponse<IList<Category>>> GetCategories();

    [Get("/api/v2/store-items/?filter=code:eq:{code}")]
    Task<IApiResponse<IList<StoreItem>>> GetStoreItem(string code);

    [Get("/api/v2/tags")]
    Task<IApiResponse<IList<HashTag>>> GetTags();

    [Post("/api/v2/transactions/make-purchase")]
    Task<IApiResponse<CreatedId>> MakePurchase([Body] MakePurchaseRequest request);

    [Get("/api/v2/cashflows/get-upcoming?from={fromDate}&to={toDate}&sort=-date")]
    Task<IApiResponse<IReadOnlyList<Upcoming>>> GetUpcomingCashflows(DateOnly fromDate, DateOnly toDate);
}
