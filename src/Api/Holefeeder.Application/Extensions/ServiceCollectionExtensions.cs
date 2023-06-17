using System.Diagnostics.CodeAnalysis;
using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Application.Mediatr;
using DrifterApps.Seeds.Domain;
using Hangfire;
using Hangfire.MemoryStorage;
using Holefeeder.Application.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Holefeeder.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfireServices();

        services.AddUserContext();

        // For all the validators, register them with dependency injection as scoped
        AssemblyScanner
            .FindValidatorsInAssembly(typeof(Application).Assembly, true)
            .ForEach(item => services.AddTransient(item.InterfaceType, item.ValidatorType));

        services.AddMemoryCache();

        services
            .AddMediatR(serviceConfiguration =>
                serviceConfiguration.RegisterServicesFromAssembly(typeof(Application).Assembly)
                    .RegisterServicesFromApplicationSeeds());

        services.AddTransient<IUnitOfWork>(provider => provider.GetRequiredService<BudgetingContext>());

        return services;
    }

    private static void AddHangfireServices(this IServiceCollection services)
    {
        // Add Hangfire services.
        services.AddHangfire(globalConfiguration => globalConfiguration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseMemoryStorage());

        // Add the processing server as IHostedService
        services.AddHangfireServer();
    }
}
