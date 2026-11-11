using Holefeeder.Service.Accounts.Api.CloseAccount;
using Holefeeder.Service.Accounts.Api.GetAccountDetail;
using Holefeeder.Service.Accounts.Api.GetAccountTransactions;
using Holefeeder.Service.Accounts.Api.ModifyAccount;
using Holefeeder.Service.Accounts.Api.OpenAccount;
using Holefeeder.Service.Accounts.Core;
using Holefeeder.Service.Accounts.Core.CloseAccount;
using Holefeeder.Service.Accounts.Core.GetAccountDetail;
using Holefeeder.Service.Accounts.Core.GetAccountTransactions;
using Holefeeder.Service.Accounts.Core.ModifyAccount;
using Holefeeder.Service.Accounts.Core.OpenAccount;
using Holefeeder.Service.Accounts.Persistence;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Holefeeder.Service.Accounts;

internal static class Accounts
{
    public static IServiceCollection AddAccountsServices(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        return services
            .AddScoped<IAccounts, AccountsInDatabase>()
            .AddScoped<GetAccountDetailUseCase>()
            .AddScoped<GetAccountTransactionsUseCase>()
            .AddScoped<OpenAccountUseCase>()
            .AddScoped<ModifyAccountUseCase>()
            .AddScoped<CloseAccountUseCase>();
    }

    public static IEndpointRouteBuilder MapAccountsEndpoints(this IEndpointRouteBuilder app) =>
        app.MapGetAccountDetail()
            .MapGetAccountTransactions()
            .MapOpenAccount()
            .MapModifyAccount()
            .MapCloseAccount();
}
