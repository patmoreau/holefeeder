using Holefeeder.Application.Features.Accounts.Commands;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.Features.Statistics.Queries;
using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.Application.Features.Tags.Queries;
using Holefeeder.Application.Models;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.UseCases.Dashboard;
using Holefeeder.Domain.ValueObjects;

using Refit;

using AccountViewModel = Holefeeder.Application.Features.Accounts.Queries.AccountViewModel;

namespace Holefeeder.FunctionalTests.Infrastructure;

public interface IUnauthenticatedUser : IUser;

public interface IForbiddenUser : IUser;

public interface IUser : IAccounts, ICategories, IEnumerations, IMyData, IPeriods, IDashboard, IStatistics, IStoreItems, ITags, ITransactions;

public interface IAccounts
{
    [Post("/api/v2/accounts/close-account")]
    Task<IApiResponse> CloseAccountAsync([Body] CloseAccount.Request request);

    [Post("/api/v2/accounts/favorite-account")]
    Task<IApiResponse> FavoriteAccountAsync([Body] FavoriteAccount.Request request);

    [Post("/api/v2/accounts/modify-account")]
    Task<IApiResponse> ModifyAccountAsync([Body] object request);

    [Post("/api/v2/accounts/open-account")]
    Task<IApiResponse> OpenAccountAsync([Body] object request);

    [Get("/api/v2/accounts/{accountId}")]
    Task<IApiResponse<AccountViewModel>> GetAccountAsync(Guid accountId);

    [Get("/api/v2/accounts")]
    Task<IApiResponse<IEnumerable<AccountViewModel>>> GetAccountsAsync([Query] int offset, [Query] int limit,
        [Query(CollectionFormat.Multi)] string[] sort, [Query(CollectionFormat.Multi)] string[] filter);
}

public interface ICategories
{
    [Get("/api/v2/categories")]
    Task<IApiResponse<IEnumerable<CategoryViewModel>>> GetCategoriesAsync();
}

public interface IEnumerations
{
    [Get("/api/v2/enumerations/get-account-types")]
    Task<IApiResponse<IEnumerable<AccountType>>> GetAccountTypesAsync();

    [Get("/api/v2/enumerations/get-category-types")]
    Task<IApiResponse<IEnumerable<CategoryType>>> GetCategoryTypesAsync();

    [Get("/api/v2/enumerations/get-date-interval-types")]
    Task<IApiResponse<IEnumerable<DateIntervalType>>> GetDateIntervalTypesAsync();
}

public interface IMyData
{
    [Post("/api/v2/my-data/import-data")]
    Task<IApiResponse> ImportDataAsync([Body] object request);

    [Get("/api/v2/my-data/export-data")]
    Task<IApiResponse<ExportDataDto>> ExportDataAsync();

    [Get("/api/v2/my-data/import-status/{importId}")]
    Task<IApiResponse<ImportDataStatusDto>> ImportDataStatusAsync(Guid importId);
}

public interface IPeriods
{
    [Get("/api/v2/periods?asOfDate={asOfDate}&iteration={iteration}")]
    Task<IApiResponse<DateInterval>> ComputePeriod(DateOnly asOfDate, int? iteration = null);
}

public interface IDashboard
{
    [Get("/api/v2/dashboard/summary")]
    Task<IApiResponse<SummaryResult>> GetDashboardSummary();
}

public interface IStatistics
{
    [Get("/api/v2/categories/statistics")]
    Task<IApiResponse<IEnumerable<StatisticsDto>>> GetStatisticsForAllCategoriesAsync();

    [Get("/api/v2/summary/statistics?from={from}&to={until}")]
    Task<IApiResponse<SummaryDto>> GetStatisticsSummaryAsync(DateOnly from, DateOnly until);
}

public interface IStoreItems
{
    [Get("/api/v2/store-items")]
    Task<IApiResponse<IEnumerable<StoreItemViewModel>>> GetStoreItemsAsync([Query] int offset, [Query] int limit,
        [Query(CollectionFormat.Multi)] string[] sort, [Query(CollectionFormat.Multi)] string[] filter);

    [Get("/api/v2/store-items/{storeItemId}")]
    Task<IApiResponse<StoreItemViewModel>> GetStoreItemAsync(Guid storeItemId);

    [Post("/api/v2/store-items/create-store-item")]
    Task<IApiResponse> CreateStoreItemAsync([Body] object request);

    [Post("/api/v2/store-items/modify-store-item")]
    Task<IApiResponse> ModifyStoreItemAsync([Body] object request);
}

public interface ITags
{
    [Get("/api/v2/tags")]
    Task<IApiResponse<IEnumerable<TagDto>>> GetTagsWithCountAsync();
}

public interface ITransactions
{
    [Get("/api/v2/cashflows")]
    Task<IApiResponse<CashflowInfoViewModel[]>> GetCashflowsAsync([Query] int offset, [Query] int limit,
        [Query(CollectionFormat.Multi)] string[] sort, [Query(CollectionFormat.Multi)] string[] filter);

    [Get("/api/v2/cashflows/{cashflowId}")]
    Task<IApiResponse<CashflowInfoViewModel>> GetCashflowAsync(Guid cashflowId);

    [Post("/api/v2/cashflows/modify")]
    Task<IApiResponse> ModifyCashflowAsync([Body] object request);

    [Post("/api/v2/cashflows/cancel")]
    Task<IApiResponse> CancelCashflowAsync([Body] object request);

    [Get("/api/v2/cashflows/get-upcoming?from={from}&to={until}&accountId={accountId}")]
    Task<IApiResponse<IEnumerable<UpcomingViewModel>>> GetUpcomingAsync(DateOnly from, DateOnly until, Guid accountId);

    [Post("/api/v2/transactions/make-purchase")]
    Task<IApiResponse> MakePurchaseAsync([Body] object request);

    [Post("/api/v2/transactions/pay-cashflow")]
    Task<IApiResponse> PayCashflowAsync([Body] object request);

    [Post("/api/v2/transactions/transfer")]
    Task<IApiResponse> TransferAsync([Body] object request);

    [Post("/api/v2/transactions/modify")]
    Task<IApiResponse> ModifyTransactionAsync([Body] object request);

    [Delete("/api/v2/transactions/{transactionId}")]
    Task<IApiResponse> DeleteTransactionAsync(Guid transactionId);

    [Get("/api/v2/transactions/{transactionId}")]
    Task<IApiResponse<TransactionInfoViewModel>> GetTransactionAsync(Guid transactionId);

    [Get("/api/v2/transactions")]
    Task<IApiResponse<IEnumerable<TransactionInfoViewModel>>> GetTransactionsAsync([Query] int offset,
        [Query] int limit, [Query(CollectionFormat.Multi)] string[] sort,
        [Query(CollectionFormat.Multi)] string[] filter);
}
