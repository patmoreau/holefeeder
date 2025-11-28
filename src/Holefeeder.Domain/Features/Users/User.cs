using System.Text.Json.Serialization;

namespace Holefeeder.Domain.Features.Users;

public sealed partial record User : IAggregateRoot
{
    private User(UserId id)
    {
        Id = id;
    }

    public UserId Id { get; }

    public bool Inactive { get; private init; }

    public IReadOnlyCollection<UserIdentity> UserIdentities { get; private init; } = [];
}

public sealed record UserId : StronglyTypedId<UserId>;
