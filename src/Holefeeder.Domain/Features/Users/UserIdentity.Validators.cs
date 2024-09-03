namespace Holefeeder.Domain.Features.Users;

public sealed partial record UserIdentity
{
    private static ResultValidation UserIdValidation(UserId id) =>
        ResultValidation.Create(() => id != UserId.Empty, UserErrors.UserIdRequired);
}
