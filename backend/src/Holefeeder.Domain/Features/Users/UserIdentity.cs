namespace Holefeeder.Domain.Features.Users;

public sealed partial record UserIdentity
{
    private UserIdentity(string identityObjectId)
    {
        IdentityObjectId = identityObjectId;
    }

    public string IdentityObjectId { get; init; }
    public bool Inactive { get; init; }

    public required User User { get; init; }
    public required UserId UserId { get; init; }
}
