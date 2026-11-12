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

    [LoggerMessage(Level = LogLevel.Error, Message = "Registered user could not be read from the database")]
    private static partial void LogReadFailed(ILogger logger, Exception exception);
}

internal static class RegisteredUsersDatabaseErrors
{
    public const string CodeReadFailed = "registered-users-read-failed";

    public static UnexpectedError ReadFailed => new(CodeReadFailed, "Registered user could not be read from the database.");
}
