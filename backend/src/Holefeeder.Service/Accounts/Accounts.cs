using Holefeeder.Service.Accounts.Api.GetAccountDetail;
using Holefeeder.Service.Accounts.Core;
using Holefeeder.Service.Accounts.Core.GetAccountDetail;
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
            .AddScoped<GetAccountDetailUseCase>();
    }

    public static IEndpointRouteBuilder MapAccountsEndpoints(this IEndpointRouteBuilder app) =>
        app.MapGetAccountDetail();
}
