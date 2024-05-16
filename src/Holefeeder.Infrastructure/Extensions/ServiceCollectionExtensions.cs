using System.Diagnostics.CodeAnalysis;

using Hangfire;

using Holefeeder.Application.Context;
using Holefeeder.Infrastructure.Scripts;
using Holefeeder.Infrastructure.SeedWork;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MySqlConnector;

namespace Holefeeder.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddSingleton<BudgetingConnectionStringBuilder>(_ => new BudgetingConnectionStringBuilder
        {
            ConnectionString =
                configuration.GetConnectionString(BudgetingConnectionStringBuilder.BudgetingConnectionString)!
        });

        services.AddSingleton<DbContextOptions>(OptionsBuilderOptions);
        services.AddSingleton(OptionsBuilderOptions);

        services.AddDbContextFactory<BudgetingContext>();
        services.AddDbContext<BudgetingContext>();

        services.AddScoped<Script000InitDatabase>();

        services.AddHangfireServices();

        return services;

        DbContextOptions<BudgetingContext> OptionsBuilderOptions(IServiceProvider provider)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BudgetingContext>();

            var connectionStringBuilder = provider.GetRequiredService<BudgetingConnectionStringBuilder>();
            var connectionString = connectionStringBuilder.CreateBuilder(MySqlGuidFormat.Binary16).ConnectionString;
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return optionsBuilder.Options;
        }
    }

    private static void AddHangfireServices(this IServiceCollection services)
    {
        // Add Hangfire services.
        services.AddHangfire(globalConfiguration => globalConfiguration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseInMemoryStorage());

        // Add the processing server as IHostedService
        services.AddHangfireServer(options => { options.WorkerCount = 1; });
    }
}
