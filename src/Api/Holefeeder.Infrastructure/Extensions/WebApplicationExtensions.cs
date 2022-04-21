using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Reflection;

using DbUp;
using DbUp.MySql;

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
            logger.LogInformation("{DatabaseName} - Migration attempt #{TryCount}", name, tryCount);
            try
            {
                PerformMigration(databaseSettings, name);
                completed = true;
            }
            catch (SocketException e)
            {
                logger.LogInformation("{DatabaseName} - Migration attempt #{TryCount} - error {Error}", name, tryCount,
                    e);
                Task.Delay(10000);
            }
        }

        if (!completed)
        {
            throw new Exception($"{name} - Unable to perform database migration, no connection to server was found.");
        }

        logger.LogInformation("{DatabaseName} - Migration completed successfully", name);
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
                    s => s.Contains($"{nameof(Scripts)}.{name}"))
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
    }
}
