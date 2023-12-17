using System.Diagnostics.CodeAnalysis;
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
            ConnectionString = configuration.GetConnectionString("BudgetingConnectionString")!
        });

        services.AddDbContext<BudgetingContext>((provider, builder) =>
        {
            var connectionStringBuilder = provider.GetRequiredService<BudgetingConnectionStringBuilder>();
            string connectionString = connectionStringBuilder.CreateBuilder(MySqlGuidFormat.Binary16).ConnectionString;
            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });

        services.AddScoped<Script000InitDatabase>();

        return services;
    }
}
