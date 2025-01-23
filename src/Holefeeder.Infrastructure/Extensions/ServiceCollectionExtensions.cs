using System.Diagnostics.CodeAnalysis;

using Hangfire;
using Hangfire.PostgreSql;

using Holefeeder.Application.Context;
using Holefeeder.Infrastructure.SeedWork;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        return services;

        DbContextOptions<BudgetingContext> OptionsBuilderOptions(IServiceProvider provider)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BudgetingContext>();

            var connectionStringBuilder = provider.GetRequiredService<BudgetingConnectionStringBuilder>();
            var connectionString = connectionStringBuilder.CreateBuilder().ConnectionString;
            optionsBuilder.UseNpgsql(connectionString);

            return optionsBuilder.Options;
        }
    }

    public static void AddHangfireServices(this IServiceCollection services)
    {
        // Add Hangfire services.
        services.AddHangfire((provider, globalConfiguration) =>
            globalConfiguration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options =>
                {
                    var builder = provider.GetRequiredService<BudgetingConnectionStringBuilder>();
                    options.UseNpgsqlConnection(builder.CreateBuilder().ConnectionString);
                }));

        // Add the processing server as IHostedService
        services.AddHangfireServer(options => { options.WorkerCount = 1; });
    }
}
