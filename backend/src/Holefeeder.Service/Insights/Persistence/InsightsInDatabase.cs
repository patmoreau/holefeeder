using Dapper;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Insights.Core;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Shared.Persistence;

using Npgsql;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Insights.Persistence;

internal sealed partial class InsightsInDatabase(NpgsqlDataSource dataSource, ILogger<InsightsInDatabase> logger) : IInsights
{
    public async Task<Result<PeriodSettings>> GetPeriodSettingsAsync(User user, DateOnly today, CancellationToken cancellationToken)
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

            return data is null ? PeriodSettings.Default(today) : SettingsData.ToPeriodSettings(data, InsightsErrors.SettingsInvalid);
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return InsightsErrors.ReadFailed;
        }
    }

    public async Task<Result<IReadOnlyList<InsightCategory>>> GetCategoriesAsync(User user, CategoryType type, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT id, name, color, budget_amount
                           FROM categories
                           WHERE user_id = @UserId AND type = @Type AND NOT system;
                           """;
        var parameters = new DynamicParameters(new { UserId = user.Id.Value, Type = type.ToString() });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var rows = await connection.QueryAsync<InsightCategoryRow>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            return rows.Select(row => row.ToInsightCategory()).ToList();
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return InsightsErrors.ReadFailed;
        }
    }

    public async Task<Result<IReadOnlyList<InsightEntry>>> GetEntriesAsync(User user, CategoryType type, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT t.date, t.category_id, t.amount, COALESCE(t.tags, '') AS tags
                           FROM transactions t
                           INNER JOIN categories c ON c.id = t.category_id
                           WHERE c.user_id = @UserId AND c.type = @Type AND NOT c.system;
                           """;
        var parameters = new DynamicParameters(new { UserId = user.Id.Value, Type = type.ToString() });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var rows = await connection.QueryAsync<InsightEntryRow>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            return rows.Select(row => row.ToInsightEntry()).ToList();
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return InsightsErrors.ReadFailed;
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Insights could not be read from the database")]
    private static partial void LogReadFailed(ILogger logger, Exception exception);
}

internal static class InsightsErrors
{
    public const string CodeReadFailed = "insights-read-failed";
    public const string CodeSettingsInvalid = "insights-settings-invalid";

    public static UnexpectedError ReadFailed => new(CodeReadFailed, "Insights could not be read from the database.");

    public static UnexpectedError SettingsInvalid => new(CodeSettingsInvalid, "User settings could not be read.");
}
