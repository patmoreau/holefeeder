using System.Net.Http.Headers;
using System.Reflection;

using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.FluentResult;
using DrifterApps.Seeds.Testing.Infrastructure.Authentication;

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
using Holefeeder.FunctionalTests.Infrastructure;

using MediatR;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using NSubstitute;

using Refit;

using AccountViewModel = Holefeeder.Application.Features.Accounts.Queries.AccountViewModel;

namespace Holefeeder.FunctionalTests.Drivers;

public class ApiApplicationSecurityDriver() : ApiApplicationDriver(false)
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseEnvironment("SECURITY_TESTS");
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<IForbiddenUser>(_ =>
            {
                var httpClient = CreateClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", new JwtTokenBuilder().Build());
                return RestService.For<IForbiddenUser>(httpClient);
            });
            services.AddSingleton<IUnauthenticatedUser>(_ =>
            {
                var httpClient = CreateClient();
                return RestService.For<IUnauthenticatedUser>(httpClient);
            });
            ReplaceHandler<CloseAccount.Request, Result<Nothing>>(services);
            ReplaceHandler<FavoriteAccount.Request, Result<Nothing>>(services);
            ReplaceHandler<ModifyAccount.Request, Result<Nothing>>(services);
            ReplaceHandler<OpenAccount.Request, Result<AccountId>>(services);
            ReplaceHandler<GetAccount.Request, Result<AccountViewModel>>(services);
            ReplaceHandler<GetAccounts.Request, Result<QueryResult<AccountViewModel>>>(services);
            ReplaceHandler<GetCategories.Request, Result<QueryResult<CategoryViewModel>>>(services);
            ReplaceHandler<GetAccountTypes.Request, IReadOnlyCollection<AccountType>>(services);
            ReplaceHandler<GetCategoryTypes.Request, IReadOnlyCollection<CategoryType>>(services);
            ReplaceHandler<GetDateIntervalTypes.Request, IReadOnlyCollection<DateIntervalType>>(services);
            ReplaceHandler<ImportData.InternalRequest, Unit>(services);
            ReplaceHandler<ExportData.Request, ExportDataDto>(services);
            ReplaceHandler<ImportDataStatus.Request, ImportDataStatusDto>(services);
            ReplaceHandler<GetForAllCategories.Request, IEnumerable<StatisticsDto>>(services);
            ReplaceHandler<GetSummary.Request, SummaryDto>(services);
            ReplaceHandler<CreateStoreItem.Request, Result<StoreItemId>>(services);
            ReplaceHandler<ModifyStoreItem.Request, Result<Nothing>>(services);
            ReplaceHandler<GetStoreItem.Request, Result<StoreItemViewModel>>(services);
            ReplaceHandler<GetStoreItems.Request, Result<QueryResult<GetStoreItems.Response>>>(services);
            ReplaceHandler<GetTagsWithCount.Request, IEnumerable<TagDto>>(services);
            ReplaceHandler<CancelCashflow.Request, Result<Nothing>>(services);
            ReplaceHandler<DeleteTransaction.Request, Result<Nothing>>(services);
            ReplaceHandler<MakePurchase.Request, Result<TransactionId>>(services);
            ReplaceHandler<ModifyCashflow.Request, Result<Nothing>>(services);
            ReplaceHandler<ModifyTransaction.Request, Result<Nothing>>(services);
            ReplaceHandler<PayCashflow.Request, Result<TransactionId>>(services);
            ReplaceHandler<Transfer.Request, Result<(TransactionId FromTransactionId, TransactionId ToTransactionId)>>(
                services);
            ReplaceHandler<GetCashflow.Request, Result<CashflowInfoViewModel>>(services);
            ReplaceHandler<GetCashflows.Request, Result<QueryResult<CashflowInfoViewModel>>>(services);
            ReplaceHandler<GetTransaction.Request, Result<TransactionInfoViewModel>>(services);
            ReplaceHandler<GetTransactions.Request, Result<QueryResult<TransactionInfoViewModel>>>(services);
            ReplaceHandler<GetUpcoming.Request, QueryResult<UpcomingViewModel>>(services);
        });
    }

    private static void ReplaceHandler<TRequest, TResponse>(IServiceCollection services,
        Action<IRequestHandler<TRequest, TResponse>>? options = null)
        where TRequest : IRequest<TResponse>  =>
        services.Replace(ServiceDescriptor.Transient<IRequestHandler<TRequest, TResponse>>(_ =>
        {
            var mockHandler = Substitute.For<IRequestHandler<TRequest, TResponse>>();
            var objType = typeof(TResponse);
            if(objType.IsGenericType && objType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var resultType = typeof(Result<>).MakeGenericType(objType.GetGenericArguments()[0]);
                var successMethod = resultType.GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .FirstOrDefault(m => m.Name == "Success" && m.GetParameters().Length == 0);
                var result = successMethod?.Invoke(null, null);

                mockHandler.Handle(Arg.Any<TRequest>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult<TResponse>((TResponse)result!));
            }
            options?.Invoke(mockHandler);
            return mockHandler;
        }));
}
