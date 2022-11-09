using System.Diagnostics.CodeAnalysis;

using Ardalis.SmartEnum.Dapper;

using Dapper;

using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Application.Features.MyData;
using Holefeeder.Application.Features.Transactions;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Infrastructure.Context;
using Holefeeder.Infrastructure.Repositories;
using Holefeeder.Infrastructure.Scripts.ObjectStore;
using Holefeeder.Infrastructure.Serializers;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

        services
            .AddOptions<ObjectStoreDatabaseSettings>()
            .Bind(configuration.GetSection(nameof(ObjectStoreDatabaseSettings)));
        services.AddOptions<HolefeederDatabaseSettings>()
            .Bind(configuration.GetSection(nameof(HolefeederDatabaseSettings)))
            .ValidateDataAnnotations();

        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<ObjectStoreDatabaseSettings>>().Value);
        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<HolefeederDatabaseSettings>>().Value);

        services.AddScoped<IHolefeederContext, HolefeederContext>();
        services.AddScoped<IObjectStoreContext, ObjectStoreContext>();
        services.AddScoped<Script000InitDatabase>();

        services.AddTransient<IAccountQueriesRepository, AccountQueriesRepository>();
        services.AddTransient<IAccountRepository, AccountRepository>();
        services.AddTransient<ICashflowQueriesRepository, CashflowQueriesRepository>();
        services.AddTransient<ICashflowRepository, CashflowRepository>();
        services.AddTransient<IMyDataQueriesRepository, MyDataQueriesRepository>();
        services.AddTransient<ITransactionQueriesRepository, TransactionQueriesRepository>();
        services.AddTransient<ITransactionRepository, TransactionRepository>();
        services.AddTransient<IUpcomingQueriesRepository, UpcomingQueriesRepository>();

        DefaultTypeMap.MatchNamesWithUnderscores = true;

        SqlMapper.AddTypeHandler(typeof(AccountType), new SmartEnumByNameTypeHandler<AccountType>());
        SqlMapper.AddTypeHandler(typeof(CategoryType), new SmartEnumByNameTypeHandler<CategoryType>());
        SqlMapper.AddTypeHandler(typeof(DateIntervalType), new SmartEnumByNameTypeHandler<DateIntervalType>());
        SqlMapper.AddTypeHandler(new TagsHandler());

        return services;
    }
}
