using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Support.Core;

internal record User
{
    public Id Id { get; }

    public IdentityObjectId IdentityObjectId { get; }

    public static Result<User> Create(Id id, IdentityObjectId identityObjectId)
    {
        if (id == Id.Empty && identityObjectId == IdentityObjectId.Empty)
        {
            return NoUser;
        }
        if (id == Id.Empty)
        {
            return UserErrors.IdRequired;
        }
        if (identityObjectId == IdentityObjectId.Empty)
        {
            return UserErrors.IdentityObjectIdRequired;
        }
        return new User(id, identityObjectId);
    }

    public static User Valid(Id id, IdentityObjectId identityObjectId) => new(id, identityObjectId);

    public static User NoUser => new(Id.Empty, IdentityObjectId.Empty);

    private User(Id id, IdentityObjectId identityObjectId) => (Id, IdentityObjectId) = (id, identityObjectId);
}

internal static class UserErrors
{
    public const string CodeIdRequired = "user-id-required";
    public const string CodeIdentityObjectIdRequired = "user-identity-object-id-required";
    public const string CodeNotRegistered = "user-not-registered";

    public static ResultError IdRequired => new(CodeIdRequired, "Id is required.");
    public static ResultError IdentityObjectIdRequired => new(CodeIdentityObjectIdRequired, "IdentityObjectId is required.");
    public static ResultError NotRegistered => new(CodeNotRegistered, "User is not registered.");
}
