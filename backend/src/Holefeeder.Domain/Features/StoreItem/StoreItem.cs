using Holefeeder.Domain.Features.Users;

namespace Holefeeder.Domain.Features.StoreItem;

public sealed partial record StoreItem : IAggregateRoot<StoreItemId>
{
    private StoreItem(StoreItemId id, string code, UserId userId)
    {
        Id = id;
        Code = code;
        UserId = userId;
    }
    public StoreItemId Id { get; }

    public string Code { get; private init; }

    public required string Data { get; init; }

    public UserId UserId { get; }
}

public sealed record StoreItemId : StronglyTypedId<StoreItemId>;

