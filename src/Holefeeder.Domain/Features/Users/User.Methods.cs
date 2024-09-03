namespace Holefeeder.Domain.Features.Users;

public sealed partial record User
{
    public Result<User> Close() => Inactive
        ? Result<User>.Failure(UserErrors.AlreadyClosed)
        : Result<User>.Success(this with { Inactive = true });
}
