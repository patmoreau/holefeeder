using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Domain.Features.Users;

public sealed partial record UserIdentity
{
    private static Func<Result<Nothing>> UserIdValidation(UserId id) =>
        () => id != UserId.Empty
            ? Result<Nothing>.Success()
            : Result<Nothing>.Failure(UserErrors.UserIdRequired);
}
