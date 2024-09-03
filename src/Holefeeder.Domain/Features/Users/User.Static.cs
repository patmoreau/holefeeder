namespace Holefeeder.Domain.Features.Users;

public sealed partial record User
{
    public static Result<User> Create() =>
        Result<User>.Success(new User(UserId.New)
        {
            Inactive = false,
        });
}
