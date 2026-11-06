using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Support.Core;

internal interface IUsers
{
    Task<Result<User>> GetByIdentityObjectId(IdentityObjectId identityObjectId, CancellationToken cancellationToken);
}
