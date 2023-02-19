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
        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        var mySqlDatabaseSettings = new BudgetingConnectionStringBuilder
        {
            ConnectionString = configuration.GetConnectionString("BudgetingConnectionString")!
        };

        services.AddSingleton(mySqlDatabaseSettings);

        services.AddDbContext<BudgetingContext>(options =>
        {
            var connectionString = mySqlDatabaseSettings.CreateBuilder(MySqlGuidFormat.Binary16).ConnectionString;
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });

        services.AddScoped<Script000InitDatabase>();

        return services;
    }
}
