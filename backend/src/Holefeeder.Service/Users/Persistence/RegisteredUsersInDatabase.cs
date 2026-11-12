using Dapper;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Users.Core;

using Npgsql;

namespace Holefeeder.Service.Users.Persistence;

internal sealed partial class RegisteredUsersInDatabase(NpgsqlDataSource dataSource, ILogger<RegisteredUsersInDatabase> logger)
    : IRegisteredUsers
{
    private const string SelectUserId =
        """
        SELECT u.id FROM users u
            JOIN user_identities i ON i.user_id = u.id
        WHERE i.identity_object_id = @IdentityObjectId
        """;

    private const string UpsertUser =
        """
        INSERT INTO users (id, inactive) VALUES (@Id, FALSE)
        ON CONFLICT (id) DO UPDATE SET inactive = EXCLUDED.inactive;
        """;

    private const string UpsertIdentity =
        """
        INSERT INTO user_identities (user_id, identity_object_id, inactive) VALUES (@Id, @IdentityObjectId, FALSE)
        ON CONFLICT (user_id, identity_object_id) DO UPDATE SET inactive = EXCLUDED.inactive;
        """;

    private const string UpsertCategory =
        """
        INSERT INTO categories (id, type, name, color, budget_amount, favorite, system, inactive, user_id)
        VALUES (@Id, @Type, @Name, @Color, 0, FALSE, TRUE, FALSE, @UserId)
        ON CONFLICT (id) DO UPDATE
        SET type = EXCLUDED.type, name = EXCLUDED.name, color = EXCLUDED.color, system = EXCLUDED.system
        WHERE categories.user_id = EXCLUDED.user_id;
        """;

    public async Task<Result<Id>> FindIdAsync(IdentityObjectId identityObjectId, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters(new { IdentityObjectId = identityObjectId.Value });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var id = await connection.QuerySingleOrDefaultAsync<Guid?>(
                new CommandDefinition(SelectUserId, parameters, cancellationToken: cancellationToken));

            return id is { } value ? Id.Valid(value) : RegisteredUserErrors.NotFound;
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return RegisteredUsersDatabaseErrors.ReadFailed;
        }
    }

    public async Task<Result<Nothing>> SaveAsync(RegisteredUser user, CancellationToken cancellationToken)
    {
        var userParameters = new { Id = user.Id.Value, IdentityObjectId = user.IdentityObjectId.Value };
        var categoryParameters = user.Categories.Select(category => new
        {
            Id = category.Id.Value,
            Type = category.Type.ToString(),
            category.Name,
            SystemCategory.Color,
            UserId = user.Id.Value
        });

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            await connection.ExecuteAsync(
                new CommandDefinition(UpsertUser, userParameters, transaction, cancellationToken: cancellationToken));
            await connection.ExecuteAsync(
                new CommandDefinition(UpsertIdentity, userParameters, transaction, cancellationToken: cancellationToken));
            await connection.ExecuteAsync(
                new CommandDefinition(UpsertCategory, categoryParameters, transaction, cancellationToken: cancellationToken));

            await transaction.CommitAsync(cancellationToken);
            return Nothing.Value;
        }
        catch (NpgsqlException exception)
        {
            LogWriteFailed(logger, exception);
            return RegisteredUsersDatabaseErrors.WriteFailed;
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Registered user could not be read from the database")]
    private static partial void LogReadFailed(ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Registered user could not be written to the database")]
    private static partial void LogWriteFailed(ILogger logger, Exception exception);
}

internal static class RegisteredUsersDatabaseErrors
{
    public const string CodeReadFailed = "registered-users-read-failed";
    public const string CodeWriteFailed = "registered-users-write-failed";

    public static UnexpectedError ReadFailed => new(CodeReadFailed, "Registered user could not be read from the database.");
    public static UnexpectedError WriteFailed => new(CodeWriteFailed, "Registered user could not be written to the database.");
}
