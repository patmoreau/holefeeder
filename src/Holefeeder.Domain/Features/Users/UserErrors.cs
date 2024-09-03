namespace Holefeeder.Domain.Features.Users;

public static class UserErrors
{
    public const string CodeAlreadyClosed = $"{nameof(User)}.{nameof(AlreadyClosed)}";
    public const string CodeUserIdRequired = $"{nameof(User)}.{nameof(UserIdRequired)}";

    public static ResultError AlreadyClosed => new(CodeAlreadyClosed, "User already closed");
    public static ResultError UserIdRequired => new(CodeUserIdRequired, "UserId is required");
}
