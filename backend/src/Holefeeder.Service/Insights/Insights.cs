using Holefeeder.Service.Insights.Api.GetSpending;
using Holefeeder.Service.Insights.Core;
using Holefeeder.Service.Insights.Core.GetSpending;
using Holefeeder.Service.Insights.Persistence;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Holefeeder.Service.Insights;

internal static class Insights
{
    public static IServiceCollection AddInsightsServices(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        return services
            .AddScoped<IInsights, InsightsInDatabase>()
            .AddScoped<GetSpendingUseCase>();
    }

    public static IEndpointRouteBuilder MapInsightsEndpoints(this IEndpointRouteBuilder app) =>
        app.MapGetSpending();
}
