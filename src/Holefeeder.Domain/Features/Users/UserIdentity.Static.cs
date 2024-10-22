using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Domain.Features.Users;

public sealed partial record UserIdentity
{
    internal static Result<UserIdentity> Create(User user, string identityObjectId)
    {
        var result = ResultAggregate.Create()
            .Ensure(UserIdValidation(user.Id));

        return result.Switch(
            () => Result<UserIdentity>.Success(new UserIdentity(identityObjectId)
            {
                IdentityObjectId = identityObjectId,
                Inactive = false,
                User = user,
                UserId = user.Id
            }),
            Result<UserIdentity>.Failure);
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
