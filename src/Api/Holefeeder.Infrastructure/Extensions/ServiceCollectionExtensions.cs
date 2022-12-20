using System.Diagnostics.CodeAnalysis;

using Ardalis.SmartEnum.Dapper;

using Dapper;

using Holefeeder.Application.Context;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Infrastructure.Scripts;
using Holefeeder.Infrastructure.SeedWork;
using Holefeeder.Infrastructure.Serializers;

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

        DefaultTypeMap.MatchNamesWithUnderscores = true;

        SqlMapper.AddTypeHandler(typeof(AccountType), new SmartEnumByNameTypeHandler<AccountType>());
        SqlMapper.AddTypeHandler(typeof(CategoryType), new SmartEnumByNameTypeHandler<CategoryType>());
        SqlMapper.AddTypeHandler(typeof(DateIntervalType), new SmartEnumByNameTypeHandler<DateIntervalType>());
        SqlMapper.AddTypeHandler(new TagsHandler());

        return services;
    }
}
