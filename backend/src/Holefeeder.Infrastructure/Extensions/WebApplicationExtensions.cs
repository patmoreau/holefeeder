using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using DbUp;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Holefeeder.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class WebApplicationExtensions
{
    private static readonly object Locker = new();

    public static void MigrateDb(this IApplicationBuilder app, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(app);

        using var scope = app.ApplicationServices.CreateScope();

        var holefeederLogger =
            scope.ServiceProvider.GetRequiredService<ILogger<BudgetingContext>>();

        MigrateDb(connectionString, holefeederLogger);
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    public static void MigrateDb(string connectionString, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        var completed = false;
        var tryCount = 0;

        while (tryCount++ < 3 && !completed)
        {
            logger.LogMigrationAttempt(tryCount);
            try
            {
                PerformMigration(connectionString);
                completed = true;
            }
            catch (Exception e)
            {
                logger.LogMigrationError(tryCount, e);
                Thread.Sleep(10000);
            }
        }

        if (!completed)
        {
            throw new DataException("Unable to perform database migration, no connection to server was found.");
        }

        logger.LogMigrationSuccess();
    }

    private static void PerformMigration(string connectionString)
    {
        lock (Locker)
        {
            var upgradeEngine = DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .WithTransactionPerScript()
                .Build();

            var result = upgradeEngine.PerformUpgrade();
            if (!result.Successful)
            {
                throw result.Error;
            }
        }
    }
}
