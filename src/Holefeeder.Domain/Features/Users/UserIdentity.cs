namespace Holefeeder.Domain.Features.Users;

public sealed record UserIdentity
{
    private UserIdentity()
    {
    }

    public required string IdentityObjectId { get; init; }
    public bool Inactive { get; init; }

    public required User User { get; init; }
    public required Guid UserId { get; init; }

    internal static UserIdentity Create(User user, string identityObjectId) =>
        new()
        {
            IdentityObjectId = identityObjectId,
            Inactive = false,
            User = user,
            UserId = user.Id
        };
}
