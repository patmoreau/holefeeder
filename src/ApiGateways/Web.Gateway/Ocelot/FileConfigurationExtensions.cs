using System.Text.RegularExpressions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Ocelot.Configuration.File;

namespace DrifterApps.Holefeeder.Web.Gateway.Ocelot
{
    public static class FileConfigurationExtensions
    {
        public static IServiceCollection ConfigureDownstreamHostAndPortsPlaceholders(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.PostConfigure<FileConfiguration>(fileConfiguration =>
            {
                var globalHosts = configuration
                    .GetSection($"{nameof(FileConfiguration.GlobalConfiguration)}:Hosts")
                    .Get<GlobalHosts>();

                foreach (var route in fileConfiguration.Routes)
                {
                    ConfigureRoute(route, globalHosts);
                }
            });

            return services;
        }

        private static void ConfigureRoute(FileRoute route, GlobalHosts globalHosts)
        {
            foreach (var hostAndPort in route.DownstreamHostAndPorts)
            {
                var host = hostAndPort.Host;
                var match = Regex.Match(host, @"^{(?<service>\w+)}$");
                if (!match.Success || !globalHosts.TryGetValue(match.Groups["service"].Value, out var uri))
                {
                    continue;
                }

                route.DownstreamScheme = uri.Scheme;
                hostAndPort.Host = uri.Host;
                hostAndPort.Port = uri.Port;
            }
        }
    }
}
