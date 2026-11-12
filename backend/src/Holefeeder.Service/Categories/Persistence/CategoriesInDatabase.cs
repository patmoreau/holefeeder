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

    public async Task<Result<Category>> GetAsync(User user, Id id, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT id, name, type, color, budget_amount, favorite, system
                           FROM categories
                           WHERE id = @Id AND user_id = @UserId AND NOT inactive;
                           """;
        var parameters = new DynamicParameters(new { Id = id.Value, UserId = user.Id.Value });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var row = await connection.QuerySingleOrDefaultAsync<CategoryRow>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            return row is null ? CategoryErrors.NotFound : row.ToCategory();
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return CategoriesErrors.ReadFailed;
        }
    }

    public async Task<Result<bool>> IsNameUsedAsync(User user, CategoryName name, Id exceptId, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT EXISTS (
                               SELECT 1
                               FROM categories
                               WHERE user_id = @UserId AND LOWER(name) = LOWER(@Name) AND id <> @ExceptId);
                           """;
        var parameters = new DynamicParameters(new { UserId = user.Id.Value, Name = name.Value, ExceptId = exceptId.Value });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            return await connection.ExecuteScalarAsync<bool>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return CategoriesErrors.ReadFailed;
        }
    }

    public async Task<Result<Nothing>> SaveAsync(User user, Category category, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO categories (id, type, name, color, budget_amount, favorite, system, inactive, user_id)
                           VALUES (@Id, @Type, @Name, @Color, @Budget, @Favorite, @System, FALSE, @UserId)
                           ON CONFLICT (id) DO UPDATE
                           SET type = EXCLUDED.type, name = EXCLUDED.name, color = EXCLUDED.color,
                               budget_amount = EXCLUDED.budget_amount, favorite = EXCLUDED.favorite
                           WHERE categories.user_id = EXCLUDED.user_id;
                           """;
        var parameters = new DynamicParameters(new
        {
            Id = category.Id.Value,
            Type = category.Type.ToString(),
            Name = category.Name.Value,
            Color = category.Color.Value,
            Budget = category.Budget.Value,
            category.Favorite,
            category.System,
            UserId = user.Id.Value
        });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var saved = await connection.ExecuteAsync(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            return saved == 0 ? CategoryErrors.NotFound : Nothing.Value;
        }
        catch (NpgsqlException exception)
        {
            LogWriteFailed(logger, exception);
            return CategoriesErrors.WriteFailed;
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Categories could not be read from the database")]
    private static partial void LogReadFailed(ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Categories could not be written to the database")]
    private static partial void LogWriteFailed(ILogger logger, Exception exception);
}

internal static class CategoriesErrors
{
    public const string CodeReadFailed = "categories-read-failed";
    public const string CodeWriteFailed = "categories-write-failed";

    public static UnexpectedError ReadFailed => new(CodeReadFailed, "Categories could not be read from the database.");

    public static UnexpectedError WriteFailed => new(CodeWriteFailed, "Categories could not be written to the database.");
}
