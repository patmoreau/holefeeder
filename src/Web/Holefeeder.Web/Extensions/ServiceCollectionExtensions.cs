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
        return new[]
        {
            new RouteConfig
            {
                RouteId = "api",
                ClusterId = "api-cluster",
                Match = new RouteMatch
                {
                    // Path or Hosts are required for each route. This catch-all pattern matches all request paths.
                    Path = "/gateway/{**remainder}"
                }
            }.WithTransformPathRemovePrefix("/gateway")
        };
    }

    private static ClusterConfig[] GetClusters(IConfiguration builderConfiguration)
    {
        return new[]
        {
            new ClusterConfig
            {
                ClusterId = "api-cluster",
                Destinations =
                    new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                    {
                        {
                            "api-cluster-destination",
                            new DestinationConfig {Address = builderConfiguration.GetValue<string>("Api:Url")}
                        }
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
            }
        };
    }
}
