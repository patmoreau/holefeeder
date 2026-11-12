using Holefeeder.Application.Features.Accounts.Commands;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.Application.Features.Users.Commands;
using Holefeeder.Application.Features.Users.Queries;
using Holefeeder.Application.Models;
using Holefeeder.Domain.UseCases.Dashboard;

using Refit;

namespace Holefeeder.FunctionalTests.Infrastructure;

public interface IUnauthenticatedUser : IUser;

public interface IForbiddenUser : IUser;

/// <summary>
///     Authenticated with a subject that has no user_identities row, which is what a
///     first-time caller looks like.
/// </summary>
public interface IUnregisteredUser : IUsers;

public interface IUser : IAccounts, IMyData, IDashboard, IStoreItems, ITransactions, IPowerSync, IUsers;

public interface IUsers
{
    [Get("/api/v2/users/me")]
    Task<IApiResponse<GetCurrentUser.Response>> GetCurrentUserAsync();

    [Post("/api/v2/users/register")]
    Task<IApiResponse<RegisterUser.Response>> RegisterUserAsync();
}

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

public interface IDashboard
{
    [Get("/api/v2/dashboard/summary")]
    Task<IApiResponse<SummaryResult>> GetDashboardSummary();
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

public interface ITransactions
{
    [Post("/api/v2/cashflows/modify")]
    Task<IApiResponse> ModifyCashflowAsync([Body] object request);

    [Post("/api/v2/transactions/modify")]
    Task<IApiResponse> ModifyTransactionAsync([Body] object request);

    [Delete("/api/v2/transactions/{transactionId}")]
    Task<IApiResponse> DeleteTransactionAsync(Guid transactionId);

    [Get("/api/v2/transactions/{transactionId}")]
    Task<IApiResponse<TransactionInfoViewModel>> GetTransactionAsync(Guid transactionId);
}

public interface IPowerSync
{
    [Post("/api/v2/sync/powersync")]
    Task<IApiResponse> SyncAsync([Body] object request);
}
