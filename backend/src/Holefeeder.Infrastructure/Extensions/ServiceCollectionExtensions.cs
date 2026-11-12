using System.Diagnostics.CodeAnalysis;

using Hangfire;
using Hangfire.PostgreSql;

using Microsoft.Extensions.DependencyInjection;

namespace Holefeeder.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static void AddHangfireServices(this IServiceCollection services,
        Func<IServiceProvider, string> connectionString)
    {
        // Add Hangfire services.
        services.AddHangfire((provider, globalConfiguration) =>
            globalConfiguration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString(provider))));

        // Add the processing server as IHostedService
        services.AddHangfireServer(options => { options.WorkerCount = 1; });
    }
}
