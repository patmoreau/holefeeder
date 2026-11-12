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
    private static readonly string[] TaggedTables = ["transactions", "cashflows"];

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

    public async Task<Result<Nothing>> RenameAsync(User user, TagName tag, TagName newTag, CancellationToken cancellationToken) =>
        await RewriteAsync(user, tag, text => TagText.Rename(text, tag, newTag), cancellationToken);

    public async Task<Result<Nothing>> RemoveAsync(User user, TagName tag, CancellationToken cancellationToken) =>
        await RewriteAsync(user, tag, text => TagText.Remove(text, tag), cancellationToken);

    private async Task<Result<Nothing>> RewriteAsync(User user, TagName tag, Func<string, string> rewrite, CancellationToken cancellationToken)
    {
        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            foreach (var table in TaggedTables)
            {
                var rows = await connection.QueryAsync<TaggedRow>(new CommandDefinition(
                    $"""
                     SELECT id, tags FROM {table}
                     WHERE user_id = @UserId
                       AND EXISTS (SELECT 1 FROM unnest(string_to_array(tags, ',')) AS tag WHERE TRIM(tag) = @Tag)
                     FOR UPDATE;
                     """,
                    new { UserId = user.Id.Value, Tag = tag.Value }, transaction, cancellationToken: cancellationToken));

                foreach (var row in rows)
                {
                    await connection.ExecuteAsync(new CommandDefinition(
                        $"UPDATE {table} SET tags = @Tags WHERE id = @Id AND user_id = @UserId;",
                        new { row.Id, Tags = rewrite(row.Tags), UserId = user.Id.Value }, transaction, cancellationToken: cancellationToken));
                }
            }

            await transaction.CommitAsync(cancellationToken);
            return Nothing.Value;
        }
        catch (NpgsqlException exception)
        {
            LogWriteFailed(logger, exception);
            return TagsErrors.WriteFailed;
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Tags could not be read from the database")]
    private static partial void LogReadFailed(ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Tags could not be written to the database")]
    private static partial void LogWriteFailed(ILogger logger, Exception exception);
}

internal static class TagsErrors
{
    public const string CodeReadFailed = "tags-read-failed";
    public const string CodeWriteFailed = "tags-write-failed";

    public static UnexpectedError ReadFailed => new(CodeReadFailed, "Tags could not be read from the database.");

    public static UnexpectedError WriteFailed => new(CodeWriteFailed, "Tags could not be written to the database.");
}
