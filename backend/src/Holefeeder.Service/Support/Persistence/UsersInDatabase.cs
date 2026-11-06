using Dapper;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Support.Core;

using Npgsql;

namespace Holefeeder.Service.Support.Persistence;

internal sealed partial class UsersInDatabase(NpgsqlDataSource dataSource, ILogger<UsersInDatabase> logger)
    : IUsers
{
    public async Task<Result<User>> GetByIdentityObjectId(IdentityObjectId identityObjectId, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters(new
        {
            IdentityObjectId = identityObjectId.Value,
        });

        const string sql =
            """
            SELECT u.id, i.identity_object_id FROM users u
                JOIN user_identities i ON i.user_id = u.id
            WHERE i.identity_object_id = @IdentityObjectId
            """;

        try
        {
            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

            var userRow = await connection.QuerySingleOrDefaultAsync<UserRow>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

            if (userRow is null)
            {
                return UsersErrors.NotFound;
            }

            return userRow.ToUser();
        }
        catch (NpgsqlException exception)
        {
            LogReadFailed(logger, exception);
            return UsersErrors.ReadFailed;
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "User could not be read from the database")]
    private static partial void LogReadFailed(ILogger logger, Exception exception);
}

internal static class UsersErrors
{
    public const string CodeReadFailed = "users-read-failed";
    public const string CodeNotFound = "users-not-found";

    public static ResultError ReadFailed => new(CodeReadFailed, "User could not be read from the database.");
    public static ResultError NotFound => new(CodeNotFound, "User was not found.");
}
