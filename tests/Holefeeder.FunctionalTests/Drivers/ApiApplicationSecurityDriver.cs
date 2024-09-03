using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Domain;

using Holefeeder.Application.Features.Accounts.Commands;
using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Application.Features.Categories.Queries;
using Holefeeder.Application.Features.Enumerations.Queries;
using Holefeeder.Application.Features.MyData.Commands;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.Features.MyData.Queries;
using Holefeeder.Application.Features.Statistics.Queries;
using Holefeeder.Application.Features.StoreItems.Commands;
using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.Application.Features.Tags.Queries;
using Holefeeder.Application.Features.Transactions.Commands;
using Holefeeder.Application.Features.Transactions.Queries;
using Holefeeder.Application.Models;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Domain.Features.Transactions;

using MediatR;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using NSubstitute;

using AccountViewModel = Holefeeder.Application.Features.Accounts.Queries.AccountViewModel;

namespace Holefeeder.FunctionalTests.Drivers;

public class ApiApplicationSecurityDriver() : ApiApplicationDriver(false)
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            ReplaceHandler(services, CloseAccountHandler);
            ReplaceHandler(services, FavoriteAccountHandler);
            ReplaceHandler(services, ModifyAccountHandler);
            ReplaceHandler(services, OpenAccountHandler);
            ReplaceHandler(services, GetAccountHandler);
            ReplaceHandler(services, GetAccountsHandler);
            ReplaceHandler(services, GetCategoriesHandler);
            ReplaceHandler(services, GetAccountTypesHandler);
            ReplaceHandler(services, GetCategoryTypesHandler);
            ReplaceHandler(services, GetDateIntervalTypesHandler);
            ReplaceHandler(services, ImportDataHandler);
            ReplaceHandler(services, ExportDataHandler);
            ReplaceHandler(services, ImportDataStatusHandler);
            ReplaceHandler(services, GetForAllCategoriesHandler);
            ReplaceHandler(services, GetSummaryHandler);
            ReplaceHandler(services, CreateStoreItemHandler);
            ReplaceHandler(services, ModifyStoreItemHandler);
            ReplaceHandler(services, GetStoreItemHandler);
            ReplaceHandler(services, GetStoreItemsHandler);
            ReplaceHandler(services, GetTagsWithCountHandler);
            ReplaceHandler(services, CancelCashflowHandler);
            ReplaceHandler(services, DeleteTransactionHandler);
            ReplaceHandler(services, MakePurchaseHandler);
            ReplaceHandler(services, ModifyCashflowHandler);
            ReplaceHandler(services, ModifyTransactionHandler);
            ReplaceHandler(services, PayCashflowHandler);
            ReplaceHandler(services, TransferHandler);
            ReplaceHandler(services, GetCashflowHandler);
            ReplaceHandler(services, GetCashflowsHandler);
            ReplaceHandler(services, GetTransactionHandler);
            ReplaceHandler(services, GetTransactionsHandler);
            ReplaceHandler(services, GetUpcomingHandler);
        });
    }

    private IRequestHandler<CloseAccount.Request, Result> CloseAccountHandler { get; } = CreateSubstitute<CloseAccount.Request, Result>();
    private IRequestHandler<FavoriteAccount.Request, Result> FavoriteAccountHandler { get; } = CreateSubstitute<FavoriteAccount.Request, Result>();
    private IRequestHandler<ModifyAccount.Request, Result> ModifyAccountHandler { get; } = CreateSubstitute<ModifyAccount.Request, Result>();
    private IRequestHandler<OpenAccount.Request, Result<AccountId>> OpenAccountHandler { get; } = CreateSubstitute<OpenAccount.Request, Result<AccountId>>();
    private IRequestHandler<GetAccount.Request, Result<AccountViewModel>> GetAccountHandler { get; } = CreateSubstitute<GetAccount.Request, Result<AccountViewModel>>();
    private IRequestHandler<GetAccounts.Request, QueryResult<AccountViewModel>> GetAccountsHandler { get; } = CreateSubstitute<GetAccounts.Request, QueryResult<AccountViewModel>>();
    private IRequestHandler<GetCategories.Request, QueryResult<CategoryViewModel>> GetCategoriesHandler { get; } = CreateSubstitute<GetCategories.Request, QueryResult<CategoryViewModel>>();
    private IRequestHandler<GetAccountTypes.Request, IReadOnlyCollection<AccountType>> GetAccountTypesHandler { get; } = CreateSubstitute<GetAccountTypes.Request, IReadOnlyCollection<AccountType>>();
    private IRequestHandler<GetCategoryTypes.Request, IReadOnlyCollection<CategoryType>> GetCategoryTypesHandler { get; } = CreateSubstitute<GetCategoryTypes.Request, IReadOnlyCollection<CategoryType>>();
    private IRequestHandler<GetDateIntervalTypes.Request, IReadOnlyCollection<DateIntervalType>> GetDateIntervalTypesHandler { get; } = CreateSubstitute<GetDateIntervalTypes.Request, IReadOnlyCollection<DateIntervalType>>();
    private IRequestHandler<ImportData.InternalRequest, Unit> ImportDataHandler { get; } = CreateSubstitute<ImportData.InternalRequest, Unit>();
    private IRequestHandler<ExportData.Request, ExportDataDto> ExportDataHandler { get; } = CreateSubstitute<ExportData.Request, ExportDataDto>();
    private IRequestHandler<ImportDataStatus.Request, ImportDataStatusDto> ImportDataStatusHandler { get; } = CreateSubstitute<ImportDataStatus.Request, ImportDataStatusDto>();
    private IRequestHandler<GetForAllCategories.Request, IEnumerable<StatisticsDto>> GetForAllCategoriesHandler { get; } = CreateSubstitute<GetForAllCategories.Request, IEnumerable<StatisticsDto>>();
    private IRequestHandler<GetSummary.Request, SummaryDto> GetSummaryHandler { get; } = CreateSubstitute<GetSummary.Request, SummaryDto>();
    private IRequestHandler<CreateStoreItem.Request, Result<StoreItemId>> CreateStoreItemHandler { get; } = CreateSubstitute<CreateStoreItem.Request, Result<StoreItemId>>();
    private IRequestHandler<ModifyStoreItem.Request, Result> ModifyStoreItemHandler { get; } = CreateSubstitute<ModifyStoreItem.Request, Result>();
    private IRequestHandler<GetStoreItem.Request, Result<StoreItemViewModel>> GetStoreItemHandler { get; } = CreateSubstitute<GetStoreItem.Request, Result<StoreItemViewModel>>();
    private IRequestHandler<GetStoreItems.Request, QueryResult<GetStoreItems.Response>> GetStoreItemsHandler { get; } = CreateSubstitute<GetStoreItems.Request, QueryResult<GetStoreItems.Response>>();
    private IRequestHandler<GetTagsWithCount.Request, IEnumerable<TagDto>> GetTagsWithCountHandler { get; } = CreateSubstitute<GetTagsWithCount.Request, IEnumerable<TagDto>>();
    private IRequestHandler<CancelCashflow.Request, Result> CancelCashflowHandler { get; } = CreateSubstitute<CancelCashflow.Request, Result>();
    private IRequestHandler<DeleteTransaction.Request, Result> DeleteTransactionHandler { get; } = CreateSubstitute<DeleteTransaction.Request, Result>();
    private IRequestHandler<MakePurchase.Request, Result<TransactionId>> MakePurchaseHandler { get; } = CreateSubstitute<MakePurchase.Request, Result<TransactionId>>();
    private IRequestHandler<ModifyCashflow.Request, Result> ModifyCashflowHandler { get; } = CreateSubstitute<ModifyCashflow.Request, Result>();
    private IRequestHandler<ModifyTransaction.Request, Result> ModifyTransactionHandler { get; } = CreateSubstitute<ModifyTransaction.Request, Result>();
    private IRequestHandler<PayCashflow.Request, Result<TransactionId>> PayCashflowHandler { get; } = CreateSubstitute<PayCashflow.Request, Result<TransactionId>>();
    private IRequestHandler<Transfer.Request, Result<(TransactionId FromTransactionId, TransactionId ToTransactionId)>> TransferHandler { get; } = CreateSubstitute<Transfer.Request, Result<(TransactionId FromTransactionId, TransactionId ToTransactionId)>>();
    private IRequestHandler<GetCashflow.Request, Result<CashflowInfoViewModel>> GetCashflowHandler { get; } = CreateSubstitute<GetCashflow.Request, Result<CashflowInfoViewModel>>();
    private IRequestHandler<GetCashflows.Request, QueryResult<CashflowInfoViewModel>> GetCashflowsHandler { get; } = CreateSubstitute<GetCashflows.Request, QueryResult<CashflowInfoViewModel>>();
    private IRequestHandler<GetTransaction.Request, Result<TransactionInfoViewModel>> GetTransactionHandler { get; } = CreateSubstitute<GetTransaction.Request, Result<TransactionInfoViewModel>>();
    private IRequestHandler<GetTransactions.Request, QueryResult<TransactionInfoViewModel>> GetTransactionsHandler { get; } = CreateSubstitute<GetTransactions.Request, QueryResult<TransactionInfoViewModel>>();
    private IRequestHandler<GetUpcoming.Request, QueryResult<UpcomingViewModel>> GetUpcomingHandler { get; } = CreateSubstitute<GetUpcoming.Request, QueryResult<UpcomingViewModel>>();

    private static IRequestHandler<TRequest, TResponse> CreateSubstitute<TRequest, TResponse>()
        where TRequest : IRequest<TResponse> => Substitute.For<IRequestHandler<TRequest, TResponse>>();

    private static void ReplaceHandler<TRequest, TResponse>(IServiceCollection services, IRequestHandler<TRequest, TResponse> handler)
        where TRequest : IRequest<TResponse> => services.Replace(ServiceDescriptor.Transient<IRequestHandler<TRequest, TResponse>>(_ => handler));
}
