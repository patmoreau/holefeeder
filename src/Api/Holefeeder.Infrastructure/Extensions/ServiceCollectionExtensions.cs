using System.Diagnostics.CodeAnalysis;

using Dapper;

using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Application.Features.Cashflows;
using Holefeeder.Application.Features.Categories;
using Holefeeder.Application.Features.MyData;
using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.Application.Features.Transactions;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Infrastructure.Context;
using Holefeeder.Infrastructure.Mapping;
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

        services.AddSingleton<AccountMapper>();
        services.AddSingleton<CashflowMapper>();
        services.AddSingleton<CategoryMapper>();
        services.AddSingleton<StoreItemMapper>();
        services.AddSingleton<TransactionMapper>();
        services.AddSingleton<TagsMapper>();

        services.AddScoped<IHolefeederContext, HolefeederContext>();
        services.AddScoped<IObjectStoreContext, ObjectStoreContext>();
        services.AddScoped<Script000InitDatabase>();

        services.AddTransient<IAccountQueriesRepository, AccountQueriesRepository>();
        services.AddTransient<IAccountRepository, AccountRepository>();
        services.AddTransient<ICashflowQueriesRepository, CashflowQueriesRepository>();
        services.AddTransient<ICashflowRepository, CashflowRepository>();
        services.AddTransient<ICategoryRepository, CategoryRepository>();
        services.AddTransient<ICategoryQueriesRepository, CategoriesQueriesRepository>();
        services.AddTransient<ICategoriesRepository, CategoriesQueriesRepository>();
        services.AddTransient<IMyDataQueriesRepository, MyDataQueriesRepository>();
        services.AddTransient<IStoreItemsQueriesRepository, StoreItemsQueriesRepository>();
        services.AddTransient<IStoreItemsRepository, StoreItemsRepository>();
        services.AddTransient<ITransactionQueriesRepository, TransactionQueriesRepository>();
        services.AddTransient<ITransactionRepository, TransactionRepository>();
        services.AddTransient<IUpcomingQueriesRepository, UpcomingQueriesRepository>();

        DefaultTypeMap.MatchNamesWithUnderscores = true;

        SqlMapper.AddTypeHandler(new AccountTypeHandler());
        SqlMapper.AddTypeHandler(new CategoryTypeHandler());
        SqlMapper.AddTypeHandler(new DateIntervalTypeHandler());
        SqlMapper.AddTypeHandler(new TagsHandler());

        return services;
    }
}
