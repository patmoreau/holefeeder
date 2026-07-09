using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Domain.Features.Users;

public sealed partial record User
{
    public Result<User> Close() => Inactive
        ? UserErrors.AlreadyClosed
        : this with { Inactive = true };
}
