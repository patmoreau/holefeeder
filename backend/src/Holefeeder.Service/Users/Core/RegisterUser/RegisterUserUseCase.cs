using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Users.Core.RegisterUser;

internal sealed class RegisterUserUseCase(IRegisteredUsers users)
{
    public async Task<Result<Id>> HandleAsync(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var existing = await users.FindIdAsync(request.IdentityObjectId, cancellationToken);
        if (existing.IsSuccess)
        {
            return RegisteredUserErrors.AlreadyRegistered;
        }
        if (existing.Error is not NotFoundError)
        {
            return existing.Error;
        }

        var user = RegisteredUser.Register(request.IdentityObjectId);
        var saved = await users.SaveAsync(user, cancellationToken);
        if (saved.IsFailure)
        {
            return saved.Error;
        }

        return user.Id;
    }
}
