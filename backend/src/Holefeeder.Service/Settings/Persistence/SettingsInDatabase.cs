using Dapper;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Settings.Core;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Shared.Persistence;

using Npgsql;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Settings.Persistence;

internal sealed partial class SettingsInDatabase(NpgsqlDataSource dataSource, ILogger<SettingsInDatabase> logger) : ISettings
{
    public async Task<Result<PeriodSettings>> GetAsync(User user, DateOnly today, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT s.data
                           FROM store_items s
                           WHERE s.user_id = @UserId AND s.code = 'settings'
                           LIMIT 1;
                           """;
        var parameters = new DynamicParameters(new { UserId = user.Id.Value });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var data = await connection.QuerySingleOrDefaultAsync<string>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            return data is null ? PeriodSettings.Default(today) : SettingsData.ToPeriodSettings(data, SettingsErrors.Invalid);
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return SettingsErrors.ReadFailed;
        }
    }

    public async Task<Result<Nothing>> SaveAsync(User user, PeriodSettings settings, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO store_items (id, code, data, user_id)
                           VALUES (@Id, 'settings', @Data, @UserId)
                           ON CONFLICT (user_id, code) DO UPDATE SET data = EXCLUDED.data;
                           """;
        var parameters = new DynamicParameters(new { Id = Guid.NewGuid(), Data = SettingsData.FromPeriodSettings(settings), UserId = user.Id.Value });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            await connection.ExecuteAsync(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            return Nothing.Value;
        }
        catch (NpgsqlException exception)
        {
            LogWriteFailed(logger, exception);
            return SettingsErrors.WriteFailed;
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to read settings from the database")]
    private static partial void LogReadFailed(ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to write settings to the database")]
    private static partial void LogWriteFailed(ILogger logger, Exception exception);
}

internal static class SettingsErrors
{
    public const string CodeReadFailed = "settings-read-failed";
    public const string CodeWriteFailed = "settings-write-failed";
    public const string CodeInvalid = "settings-invalid";

    public static UnexpectedError ReadFailed => new(CodeReadFailed, "Settings could not be read from the database.");

    public static UnexpectedError WriteFailed => new(CodeWriteFailed, "Settings could not be written to the database.");

    public static UnexpectedError Invalid => new(CodeInvalid, "Stored settings could not be read.");
}
