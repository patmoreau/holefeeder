using System.Diagnostics.CodeAnalysis;

using Dapper;

using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Infrastructure.Context;
using Holefeeder.Infrastructure.Mapping;
using Holefeeder.Infrastructure.Repositories;
using Holefeeder.Infrastructure.Scripts;

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

        services.AddSingleton<StoreItemMapper>();

        services.AddScoped<IHolefeederContext, HolefeederContext>();
        services.AddScoped<IObjectStoreContext, ObjectStoreContext>();
        services.AddScoped<Script000InitDatabase>();

        services.AddTransient<IStoreItemsQueriesRepository, StoreItemsQueriesRepository>();
        services.AddTransient<IStoreItemsRepository, StoreItemsRepository>();

        DefaultTypeMap.MatchNamesWithUnderscores = true;

        return services;
    }
}
