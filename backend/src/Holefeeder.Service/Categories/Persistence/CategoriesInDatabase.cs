using Dapper;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Categories.Core;
using Holefeeder.Service.Shared.Core;

using Npgsql;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Categories.Persistence;

internal sealed partial class CategoriesInDatabase(NpgsqlDataSource dataSource, ILogger<CategoriesInDatabase> logger) : ICategories
{
    public async Task<Result<IReadOnlyList<Category>>> ListAsync(User user, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT id, name, type, color, budget_amount, favorite, system
                           FROM categories
                           WHERE user_id = @UserId AND NOT inactive
                           ORDER BY type, favorite DESC, name;
                           """;
        var parameters = new DynamicParameters(new { UserId = user.Id.Value });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var rows = await connection.QueryAsync<CategoryRow>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            return rows.Select(row => row.ToCategory()).ToList();
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return CategoriesErrors.ReadFailed;
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Categories could not be read from the database")]
    private static partial void LogReadFailed(ILogger logger, Exception exception);
}

internal static class CategoriesErrors
{
    public const string CodeReadFailed = "categories-read-failed";

    public static UnexpectedError ReadFailed => new(CodeReadFailed, "Categories could not be read from the database.");
}
