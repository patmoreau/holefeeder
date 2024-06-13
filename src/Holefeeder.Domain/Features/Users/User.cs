namespace Holefeeder.Domain.Features.Users;

public sealed record User : IAggregateRoot
{
    private readonly Guid _id;

    public required Guid Id
    {
        get => _id;
        init
        {
            if (value.Equals(Guid.Empty))
            {
                throw new UserDomainException($"{nameof(Id)} is required");
            }

            _id = value;
        }
    }

    public bool Inactive { get; init; }

    public ICollection<UserIdentity> UserIdentities { get; init; } = [];

    public static User Create() =>
        new()
        {
            Id = Guid.NewGuid(),
            Inactive = false
        };

    public User Close()
    {
        if (Inactive)
        {
            throw new UserDomainException("User already closed");
        }

        return this with { Inactive = true };
    }
}
