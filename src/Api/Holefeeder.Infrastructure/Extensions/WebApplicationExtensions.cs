using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using DbUp;
using DbUp.MySql;

using Holefeeder.Application.Extensions;
using Holefeeder.Infrastructure.Context;
using Holefeeder.Infrastructure.SeedWork;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MySqlConnectionManager = Holefeeder.Infrastructure.SeedWork.MySqlConnectionManager;

namespace Holefeeder.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class WebApplicationExtensions
{
    private static readonly object Locker = new();

    public static IApplicationBuilder MigrateDb(this IApplicationBuilder app)
    {
        if (app is null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        using var scope = app.ApplicationServices.CreateScope();

        var objectStoreDatabaseSettings = scope.ServiceProvider.GetRequiredService<ObjectStoreDatabaseSettings>();
        var objectStoreLogger = scope.ServiceProvider.GetRequiredService<ILogger<ObjectStoreContext>>();

        MigrateDb(objectStoreDatabaseSettings, objectStoreLogger, "ObjectStore");

        var holefeederDatabaseSettings = scope.ServiceProvider.GetRequiredService<HolefeederDatabaseSettings>();
        var holefeederLogger = scope.ServiceProvider.GetRequiredService<ILogger<HolefeederContext>>();

        MigrateDb(holefeederDatabaseSettings, holefeederLogger, "Holefeeder");

        return app;
    }

    private static void MigrateDb(MySqlDatabaseSettings databaseSettings, ILogger logger, string name)
    {
        var completed = false;
        var tryCount = 0;

        while (tryCount++ < 3 && !completed)
        {
            logger.LogMigrationAttempt(name, tryCount);
            try
            {
                PerformMigration(databaseSettings, name);
                completed = true;
            }
#pragma warning disable CA1031
            catch (Exception e)
#pragma warning restore CA1031
            {
                logger.LogMigrationError(name, tryCount, e);
                Thread.Sleep(10000);
            }
        }

        if (!completed)
        {
            throw new DataException(
                $"{name} - Unable to perform database migration, no connection to server was found.");
        }

        logger.LogMigrationSuccess(name);
    }

    private static void PerformMigration(MySqlDatabaseSettings databaseSettings, string name)
    {
        lock (Locker)
        {
            var connectionManager = new MySqlConnectionManager(databaseSettings.ConnectionString);

            EnsureDatabase.For.MySqlDatabase(databaseSettings.GetBuilder(true).ConnectionString);

            var upgradeEngine = DeployChanges.To
                .MySqlDatabase(connectionManager)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(),
                    s => s.Contains($"{nameof(Scripts)}.{name}", StringComparison.OrdinalIgnoreCase))
                .JournalTo(new MySqlTableJournal(
                    () => connectionManager,
                    () => MySqlConnectionManager.Log, databaseSettings.GetBuilder().Database, "schema_versions"))
                .LogToConsole()
                .Build();

            var result = upgradeEngine.PerformUpgrade();
            if (!result.Successful)
            {
                throw result.Error;
            }
        }
    }
}
