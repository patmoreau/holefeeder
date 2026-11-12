using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Users.Core.GetCurrentUser;

internal sealed class GetCurrentUserUseCase(IRegisteredUsers users)
{
    public Task<Result<Id>> HandleAsync(GetCurrentUserRequest request, CancellationToken cancellationToken) =>
        users.FindIdAsync(request.IdentityObjectId, cancellationToken);
}
