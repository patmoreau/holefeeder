using Holefeeder.Web.Yarp;

using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

namespace Holefeeder.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddYarpReverseProxy(this IServiceCollection services,
        ConfigurationManager builderConfiguration)
    {
        services.AddReverseProxy()
            .LoadFromMemory(GetRoutes(), GetClusters(builderConfiguration));

        return services;
    }

    private static RouteConfig[] GetRoutes()
    {
        return new[] {BuildRouteConfig("budgeting"), BuildRouteConfig("object-store")};
    }

    private static RouteConfig BuildRouteConfig(string routeId)
    {
        return new RouteConfig
        {
            RouteId = routeId,
            ClusterId = $"{routeId}-cluster",
            Match = new RouteMatch
            {
                // Path or Hosts are required for each route. This catch-all pattern matches all request paths.
                Path = $"/gateway/{routeId}/{{**remainder}}"
            }
        }.WithTransformPathRemovePrefix($"/gateway/{routeId}");
    }

    private static ClusterConfig[] GetClusters(ConfigurationManager builderConfiguration)
    {
        return new[]
        {
            BuildClusterConfig("budgeting", builderConfiguration.GetValue<string>("Budgeting:Url")),
            BuildClusterConfig("object-store", builderConfiguration.GetValue<string>("ObjectStore:Url"))
        };
    }

    private static ClusterConfig BuildClusterConfig(string clusterId, string destinationUrl)
    {
        return new ClusterConfig
        {
            ClusterId = $"{clusterId}-cluster",
            Destinations =
                new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                {
                    {$"{clusterId}-cluster-destination", new DestinationConfig {Address = destinationUrl}}
                },
            HealthCheck = new HealthCheckConfig
            {
                Active = new ActiveHealthCheckConfig
                {
                    Enabled = true,
                    Interval = new TimeSpan(0, 0, 10),
                    Timeout = new TimeSpan(0, 0, 10),
                    Policy = "ConsecutiveFailures",
                    Path = "/healthz"
                }
            },
            Metadata = new Dictionary<string, string> {{"ConsecutiveFailuresHealthPolicy.Threshold", "3"}}
        };
    }
}
