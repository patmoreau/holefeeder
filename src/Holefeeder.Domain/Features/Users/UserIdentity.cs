namespace Holefeeder.Domain.Features.Users;

public sealed record class UserIdentity
{
    private UserIdentity()
    {
    }

    public required Guid Id { get; init; }
    public required string Sub { get; init; }
    public bool Inactive { get; init; }

    // Navigation property
    public required User User { get; init; }
    public required Guid UserId { get; init; }

    internal static UserIdentity Create(User user, string sub) =>
        new()
        {
            Id = user.Id,
            Sub = sub,
            Inactive = false,
            User = user,
            UserId = user.Id
        };
}
