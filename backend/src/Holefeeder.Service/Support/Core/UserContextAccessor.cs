namespace Holefeeder.Service.Support.Core;

internal sealed class UserContextAccessor
{
    public User Current { get; private set; } = User.NoUser;

    public void Set(User user) => Current = user;
}
