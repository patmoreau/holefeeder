using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Domain.Features.Users;

public static class UserErrors
{
    public const string CodeAlreadyClosed = $"{nameof(User)}.{nameof(AlreadyClosed)}";
    public const string CodeUserIdRequired = $"{nameof(User)}.{nameof(UserIdRequired)}";
    public const string CodeIdentityObjectIdRequired = $"{nameof(User)}.{nameof(IdentityObjectIdRequired)}";
    public const string CodeNotFound = $"{nameof(User)}.{nameof(NotFound)}";
    public const string CodeAlreadyRegistered = $"{nameof(User)}.{nameof(AlreadyRegistered)}";

    public static ResultError AlreadyClosed => new(CodeAlreadyClosed, "User already closed");
    public static ResultError UserIdRequired => new(CodeUserIdRequired, "UserId is required");

    public static ResultError IdentityObjectIdRequired =>
        new(CodeIdentityObjectIdRequired, "IdentityObjectId is required");

    public static ResultError NotFound => new(CodeNotFound, "User is not registered");

    public static ResultError AlreadyRegistered => new(CodeAlreadyRegistered, "User is already registered");
}
