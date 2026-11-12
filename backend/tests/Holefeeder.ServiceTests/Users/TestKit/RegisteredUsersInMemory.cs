using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Users.Core;

namespace Holefeeder.ServiceTests.Users.TestKit;

internal sealed class RegisteredUsersInMemory : IRegisteredUsers
{
    private readonly Dictionary<IdentityObjectId, Id> _users = [];
    private readonly List<RegisteredUser> _saved = [];
    private ResultError? _failure;

    public IReadOnlyList<RegisteredUser> Saved => _saved;

    public Task<Result<Id>> FindIdAsync(IdentityObjectId identityObjectId, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<Id>>(_failure);
        }

        return Task.FromResult<Result<Id>>(
            _users.TryGetValue(identityObjectId, out var id) ? id : RegisteredUserErrors.NotFound);
    }

    public Task<Result<Nothing>> SaveAsync(RegisteredUser user, CancellationToken cancellationToken)
    {
        _saved.Add(user);
        _users[user.IdentityObjectId] = user.Id;
        return Task.FromResult<Result<Nothing>>(Nothing.Value);
    }

    public void Add(IdentityObjectId identityObjectId, Id id) => _users.Add(identityObjectId, id);

    public void FailWith(ResultError failure) => _failure = failure;
}
