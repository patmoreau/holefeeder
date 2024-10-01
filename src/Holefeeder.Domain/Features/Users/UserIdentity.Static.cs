namespace Holefeeder.Domain.Features.Users;

public sealed partial record UserIdentity
{
    internal static Result<UserIdentity> Create(User user, string identityObjectId)
    {
        var result = Result.Validate(UserIdValidation(user.Id));
        if (result.IsFailure)
        {
            return Result<UserIdentity>.Failure(result.Error);
        }

        return Result<UserIdentity>.Success(new UserIdentity(identityObjectId)
        {
            IdentityObjectId = identityObjectId,
            Inactive = false,
            User = user,
            UserId = user.Id
        });
    }

    internal static UserIdentity UnsafeCreate(string identityObjectId, bool inactive, User user) =>
        new(identityObjectId)
        {
            IdentityObjectId = identityObjectId,
            Inactive = inactive,
            User = user,
            UserId = user.Id
        };
}
