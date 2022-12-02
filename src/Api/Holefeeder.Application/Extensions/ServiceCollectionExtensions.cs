using System.Diagnostics.CodeAnalysis;

using Hangfire;
using Hangfire.MemoryStorage;

using Holefeeder.Application.Behaviors;
using Holefeeder.Application.Context;
using Holefeeder.Application.Features.MyData.Commands;
using Holefeeder.Application.SeedWork;
using Holefeeder.Application.SeedWork.BackgroundRequest;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Holefeeder.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var storeItemConnectionString = configuration.GetConnectionString("ObjectStoreConnectionString");
        services.AddDbContext<StoreItemContext>(options =>
        {
            options.UseMySql(storeItemConnectionString, ServerVersion.AutoDetect(storeItemConnectionString));
        });

        var holefeederConnectionString = configuration.GetConnectionString("HolefeederConnectionString");
        services.AddDbContext<BudgetingContext>(options =>
        {
            options.UseMySql(holefeederConnectionString, ServerVersion.AutoDetect(holefeederConnectionString));
        });

        services.AddHangfireServices();

        services.AddHttpContextAccessor();
        services.AddTransient<IUserContext, UserContext>()
            .AddScoped<CommandsScheduler>()
            .AddScoped<CommandsExecutor>();

        // For all the validators, register them with dependency injection as scoped
        AssemblyScanner
            .FindValidatorsInAssembly(typeof(Application).Assembly, true)
            .ForEach(item => services.AddTransient(item.InterfaceType, item.ValidatorType));

        services.AddMemoryCache();

        services
            .AddMediatR(typeof(Application).Assembly)
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        //services.AddGraphQLServer().AddQueryType<Query>().AddProjections().AddFiltering().AddSorting();

        return services;
    }

    private static IServiceCollection AddHangfireServices(this IServiceCollection services)
    {
        // Add Hangfire services.
        services.AddHangfire(globalConfiguration => globalConfiguration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseMemoryStorage());

        // Add the processing server as IHostedService
        services.AddHangfireServer();

        return services;
    }
}
