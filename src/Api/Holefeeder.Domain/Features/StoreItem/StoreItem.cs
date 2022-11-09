using Holefeeder.Application.Domain.StoreItem;
using Holefeeder.Domain.SeedWork;

namespace Holefeeder.Domain.Features.StoreItem;

public sealed record StoreItem : Entity, IAggregateRoot
{
    private readonly string _code = null!;
    private readonly Guid _id;
    private readonly Guid _userId;

    public StoreItem(Guid id, string code, Guid userId)
    {
        Id = id;
        Code = code;
        UserId = userId;
    }

    public override Guid Id
    {
        get => _id;
        init
        {
            if (value.Equals(default))
            {
                throw new ObjectStoreDomainException($"'{nameof(Id)}' is required");
            }

            _id = value;
        }
    }

    public string Code
    {
        get => _code;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ObjectStoreDomainException($"'{nameof(Code)}' is required");
            }

            _code = value;
        }
    }

    public string Data { get; init; } = string.Empty;

    public Guid UserId
    {
        get => _userId;
        init
        {
            if (value.Equals(default))
            {
                throw new ObjectStoreDomainException($"'{nameof(UserId)}' is required");
            }

            _userId = value;
        }
    }

    public static StoreItem Create(string code, string data, Guid userId)
    {
        return new StoreItem(Guid.NewGuid(), code, userId) {Data = data};
    }
}
