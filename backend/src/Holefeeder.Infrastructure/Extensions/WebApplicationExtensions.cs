using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using DbUp;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Infrastructure.SeedWork;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Holefeeder.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class WebApplicationExtensions
{
    private static readonly object Locker = new();

    public static void MigrateDb(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        using var scope = app.ApplicationServices.CreateScope();

        var connectionStringBuilder =
            scope.ServiceProvider.GetRequiredService<BudgetingConnectionStringBuilder>();
        var holefeederLogger =
            scope.ServiceProvider.GetRequiredService<ILogger<BudgetingContext>>();

        MigrateDb(connectionStringBuilder, holefeederLogger);
    }

    public static void MigrateDb(this DatabaseFacade databaseFacade,
        BudgetingConnectionStringBuilder connectionStringBuilder)
    {
        ArgumentNullException.ThrowIfNull(databaseFacade);

        using var loggerFactory = new NullLoggerFactory();
        var holefeederLogger = new Logger<BudgetingContext>(loggerFactory);

        MigrateDb(connectionStringBuilder, holefeederLogger);
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
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

    private static void PerformMigration(BudgetingConnectionStringBuilder connectionStringBuilder)
    {
        lock (Locker)
        {
            var builder = connectionStringBuilder.CreateBuilder();

            var upgradeEngine = DeployChanges.To
                .PostgresqlDatabase(builder.ConnectionString)
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
