using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Support.Persistence;

namespace Holefeeder.ServiceTests.Support.TestKit;

internal sealed class UsersInMemory : IUsers
{
    private readonly Dictionary<IdentityObjectId, User> _users = new();

    public Task<Result<User>> GetByIdentityObjectId(IdentityObjectId identityObjectId, CancellationToken cancellationToken)
    {
        if (_users.TryGetValue(identityObjectId, out var user))
        {
            return Task.FromResult<Result<User>>(user);
        }

        return Task.FromResult<Result<User>>(UsersErrors.NotFound);
    }

    public void Add(User user) => _users.Add(user.IdentityObjectId, user);
}
