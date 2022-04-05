using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Reflection;

using DbUp;
using DbUp.MySql;

using Holefeeder.Infrastructure.Context;

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

        var databaseSettings = scope.ServiceProvider.GetRequiredService<ObjectStoreDatabaseSettings>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ObjectStoreContext>>();

        var completed = false;
        var tryCount = 0;

        while (tryCount++ < 3 && !completed)
        {
            logger.LogInformation("Migration attempt #{TryCount}", tryCount);
            try
            {
                PerformMigration(databaseSettings);
                completed = true;
            }
            catch (SocketException e)
            {
                logger.LogInformation("Migration attempt #{TryCount} - error {Error}", tryCount, e);
                Task.Delay(10000);
            }
        }

        if (!completed)
        {
            throw new Exception("Unable to perform database migration, no connection to server was found.");
        }

        logger.LogInformation("Migration completed successfully");

        return app;
    }

    private static void PerformMigration(ObjectStoreDatabaseSettings databaseSettings)
    {
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
    }
}
