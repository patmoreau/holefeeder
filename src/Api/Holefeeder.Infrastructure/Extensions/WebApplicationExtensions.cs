using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using DbUp;
using DbUp.MySql;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Infrastructure.SeedWork;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MySqlConnector;

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

        var connectionStringBuilder = scope.ServiceProvider.GetRequiredService<BudgetingConnectionStringBuilder>();
        var holefeederLogger = scope.ServiceProvider.GetRequiredService<ILogger<BudgetingContext>>();

        MigrateDb(connectionStringBuilder, holefeederLogger);

        return app;
    }

    private static void MigrateDb(BudgetingConnectionStringBuilder connectionStringBuilder, ILogger logger)
    {
        var completed = false;
        var tryCount = 0;

        while (tryCount++ < 3 && !completed)
        {
            logger.LogMigrationAttempt(tryCount);
            try
            {
                PerformMigration(connectionStringBuilder);
                completed = true;
            }
#pragma warning disable CA1031
            catch (Exception e)
#pragma warning restore CA1031
            {
                logger.LogMigrationError(tryCount, e);
                Thread.Sleep(10000);
            }
        }

        if (!completed)
        {
            throw new DataException(
                $"Unable to perform database migration, no connection to server was found.");
        }

        logger.LogMigrationSuccess();
    }

    private static void PerformMigration(BudgetingConnectionStringBuilder connectionStringBuilder)
    {
        lock (Locker)
        {
            var builder = connectionStringBuilder.CreateBuilder();

            var connectionManager = new MySqlConnectionManager(connectionStringBuilder);

            EnsureDatabase.For.MySqlDatabase(builder.ConnectionString);

            var upgradeEngine = DeployChanges.To
                .MySqlDatabase(connectionManager)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .JournalTo(new MySqlTableJournal(
                    () => connectionManager,
                    () => MySqlConnectionManager.Log, builder.Database, "schema_versions"))
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
