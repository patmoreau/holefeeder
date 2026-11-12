using Dapper;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Tags.Core;
using Holefeeder.Service.Tags.Core.ListTags;

using Npgsql;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Tags.Persistence;

internal sealed partial class TagsInDatabase(NpgsqlDataSource dataSource, ILogger<TagsInDatabase> logger) : ITags
{
    public async Task<Result<IReadOnlyList<TagSummary>>> ListAsync(User user, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT tag, COUNT(*) FILTER (WHERE source = 'transaction') AS transaction_count,
                                  COUNT(*) FILTER (WHERE source = 'cashflow') AS active_cashflow_count
                           FROM (SELECT DISTINCT t.id, 'transaction' AS source, TRIM(tag) AS tag
                                 FROM transactions t, unnest(string_to_array(t.tags, ',')) AS tag
                                 WHERE t.user_id = @UserId
                                 UNION ALL
                                 SELECT DISTINCT c.id, 'cashflow' AS source, TRIM(tag) AS tag
                                 FROM cashflows c, unnest(string_to_array(c.tags, ',')) AS tag
                                 WHERE c.user_id = @UserId AND NOT c.inactive) usages
                           WHERE tag <> ''
                           GROUP BY tag
                           ORDER BY tag;
                           """;
        var parameters = new DynamicParameters(new { UserId = user.Id.Value });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var rows = await connection.QueryAsync<TagSummaryRow>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            return rows.Select(row => row.ToTagSummary()).ToList();
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return TagsErrors.ReadFailed;
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Tags could not be read from the database")]
    private static partial void LogReadFailed(ILogger logger, Exception exception);
}

internal static class TagsErrors
{
    public const string CodeReadFailed = "tags-read-failed";

    public static UnexpectedError ReadFailed => new(CodeReadFailed, "Tags could not be read from the database.");
}
