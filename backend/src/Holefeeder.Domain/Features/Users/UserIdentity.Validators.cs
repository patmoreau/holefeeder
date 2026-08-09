using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Domain.Features.Users;

public sealed partial record UserIdentity
{
    private static Func<Result<Nothing>> UserIdValidation(UserId id) =>
        () => id != UserId.Empty
            ? Nothing.Value
            : UserErrors.UserIdRequired;

    private static Func<Result<Nothing>> IdentityObjectIdValidation(string identityObjectId) =>
        () => !string.IsNullOrWhiteSpace(identityObjectId)
            ? Nothing.Value
            : UserErrors.IdentityObjectIdRequired;
}
