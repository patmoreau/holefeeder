using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using DbUp;

namespace Holefeeder.Service.Support.Persistence.Schema;

[ExcludeFromCodeCoverage]
internal static partial class SchemaMigrations
{
    private const string ScriptsPrefix = "Holefeeder.Infrastructure.Scripts.";

    private static readonly Lock Locker = new();

    public static void MigrateDb(this IApplicationBuilder app, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(app);

        var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(SchemaMigrations));

        MigrateDb(connectionString, logger);
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    public static void MigrateDb(string connectionString, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        var completed = false;
        var tryCount = 0;

        while (tryCount++ < 3 && !completed)
        {
            LogMigrationAttempt(logger, tryCount);
            try
            {
                PerformMigration(connectionString);
                completed = true;
            }
            catch (Exception e)
            {
                LogMigrationError(logger, tryCount, e);
                Thread.Sleep(10000);
            }
        }

        if (!completed)
        {
            throw new DataException("Unable to perform database migration, no connection to server was found.");
        }

        LogMigrationSuccess(logger);
    }

    private static void PerformMigration(string connectionString)
    {
        lock (Locker)
        {
            var upgradeEngine = DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(),
                    name => name.StartsWith(ScriptsPrefix, StringComparison.Ordinal))
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

    [LoggerMessage(EventId = 1007, Level = LogLevel.Information, Message = "Migration attempt #{TryCount}")]
    private static partial void LogMigrationAttempt(ILogger logger, int tryCount);

    [LoggerMessage(EventId = 1008, Level = LogLevel.Information, Message = "Migration completed successfully")]
    private static partial void LogMigrationSuccess(ILogger logger);

    [LoggerMessage(EventId = 9000, Level = LogLevel.Error, Message = "Migration attempt #{TryCount}")]
    private static partial void LogMigrationError(ILogger logger, int tryCount, Exception exception);
}
