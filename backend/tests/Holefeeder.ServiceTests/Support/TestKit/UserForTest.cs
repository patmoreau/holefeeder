using Dapper;

using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.Support.TestKit;

internal static class UserForTest
{
    private const string InsertUser = "INSERT INTO users (id, inactive) VALUES (@Id, FALSE);";

    private const string InsertIdentity =
        "INSERT INTO user_identities (user_id, identity_object_id, inactive) VALUES (@Id, @IdentityObjectId, FALSE);";

    internal static User AUser() => User.Valid(AnId(), AnIdentityObjectId());

    public static async Task<User> SavedInDbAsync(this User user, DatabaseForTest database)
    {
        var parameters = new DynamicParameters(new
        {
            Id = user.Id.Value,
            IdentityObjectId = user.IdentityObjectId.Value,
        });

        await using var connection = database.DataSource.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(InsertUser, parameters));
        await connection.ExecuteAsync(new CommandDefinition(InsertIdentity, parameters));
        return user;
    }
}
