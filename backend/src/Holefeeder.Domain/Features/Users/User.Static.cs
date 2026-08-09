using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Domain.Features.Users;

public sealed partial record User
{
    public static Result<User> Create() =>
        new User(UserId.New)
        {
            Inactive = false,
        };

    public static Result<User> Register(string identityObjectId)
    {
        // The identity needs the user it belongs to, and the user needs to expose that
        // identity — with a `with` expression the identity would point at a discarded
        // copy, and EF, which maps UserIdentities as a navigation, would insert a
        // second user. Handing the collection in up front keeps it a single instance.
        var identities = new List<UserIdentity>();
        var user = new User(UserId.New) { Inactive = false, UserIdentities = identities };

        var result = UserIdentity.Create(user, identityObjectId);
        if (result.IsFailure)
        {
            return result.Error;
        }

        identities.Add(result.Value);

        return user;
    }
}
