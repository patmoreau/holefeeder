using Holefeeder.Service.Dashboard.Api.GetDashboardAccounts;
using Holefeeder.Service.Dashboard.Api.GetDashboardSummary;
using Holefeeder.Service.Dashboard.Core;
using Holefeeder.Service.Dashboard.Core.GetDashboardAccounts;
using Holefeeder.Service.Dashboard.Core.GetDashboardSummary;
using Holefeeder.Service.Dashboard.Persistence;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Holefeeder.Service.Dashboard;

internal static class Dashboard
{
    public static IServiceCollection AddDashboardServices(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        return services
            .AddScoped<IDashboard, DashboardInDatabase>()
            .AddScoped<GetDashboardSummaryUseCase>()
            .AddScoped<GetDashboardAccountsUseCase>();
    }

    public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder app) =>
        app.MapGetDashboardSummary()
            .MapGetDashboardAccounts();
}
