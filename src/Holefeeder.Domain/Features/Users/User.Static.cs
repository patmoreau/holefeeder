using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Domain.Features.Users;

public sealed partial record User
{
    public static Result<User> Create() =>
        new User(UserId.New)
        {
            Inactive = false,
        };
}
