using System;
using System.Reflection;

using Dapper;

using DbUp;
using DbUp.MySql;

using DrifterApps.Holefeeder.ObjectStore.Application.StoreItems;
using DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Context;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Repositories;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Scripts;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using MySqlConnectionManager = Framework.Dapper.SeedWork.MySqlConnectionManager;

namespace DrifterApps.Holefeeder.ObjectStore.Infrastructure;

public static class ServiceCollectionExtensions
{
    private static readonly object Locker = new();

    public static IServiceCollection AddObjectStoreDatabase(this IServiceCollection services,
        IConfiguration configuration)
    {
        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        services.AddOptions<ObjectStoreDatabaseSettings>()
            .Bind(configuration.GetSection(nameof(ObjectStoreDatabaseSettings)))
            .ValidateDataAnnotations();
            
        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<ObjectStoreDatabaseSettings>>().Value);

        services.AddScoped<IObjectStoreContext, ObjectStoreContext>();
        services.AddScoped<Script000InitDatabase>();

        services.AddTransient<IStoreItemsQueriesRepository, StoreItemsQueriesRepository>();
        services.AddTransient<IStoreItemsRepository, StoreItemsRepository>();

        services.AddAutoMapper(typeof(MappingProfile));

        DefaultTypeMap.MatchNamesWithUnderscores = true;

        return services;
    }

    public static IApplicationBuilder MigrateDb(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var databaseSettings = scope.ServiceProvider.GetRequiredService<ObjectStoreDatabaseSettings>();

        lock (Locker)
        {
            var connectionManager =
                new MySqlConnectionManager(databaseSettings.ConnectionString);

            EnsureDatabase.For.MySqlDatabase(databaseSettings.GetBuilder(true).ConnectionString);

            var upgradeEngine = DeployChanges.To
                .MySqlDatabase(connectionManager)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .JournalTo(new MySqlTableJournal(
                    () => connectionManager,
                    () => MySqlConnectionManager.Log, databaseSettings.GetBuilder().Database,
                    "schema_versions"))
                .LogToConsole()
                .Build();

            var result = upgradeEngine.PerformUpgrade();
            if (!result.Successful)
            {
                throw result.Error;
            }
        }

        return app;
    }
}
