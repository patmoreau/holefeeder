using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Users.Core;

internal interface IRegisteredUsers
{
    Task<Result<Id>> FindIdAsync(IdentityObjectId identityObjectId, CancellationToken cancellationToken);

    Task<Result<Nothing>> SaveAsync(RegisteredUser user, CancellationToken cancellationToken);
}
