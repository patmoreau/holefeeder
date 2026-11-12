using Microsoft.Extensions.Logging;

namespace Holefeeder.Application.Extensions;

public static partial class LoggerExtensions
{
    [LoggerMessage(
        EventId = 1007,
        Level = LogLevel.Information,
        Message = "Migration attempt #{TryCount}")]
    public static partial void LogMigrationAttempt(this ILogger logger, int tryCount);

    [LoggerMessage(
        EventId = 1008,
        Level = LogLevel.Information,
        Message = "Migration completed successfully")]
    public static partial void LogMigrationSuccess(this ILogger logger);

    [LoggerMessage(
        EventId = 9000,
        Level = LogLevel.Error,
        Message = "Migration attempt #{TryCount}")]
    public static partial void LogMigrationError(this ILogger logger, int tryCount, Exception exception);
}
