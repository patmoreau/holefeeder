namespace Holefeeder.Domain.Features.StoreItem;

public sealed record StoreItem : IAggregateRoot
{
    private readonly string _code = null!;
    private readonly Guid _id;
    private readonly Guid _userId;

    public required Guid Id
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

    public required string Code
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

    public string Data { get; set; } = string.Empty;

    public required Guid UserId
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

    public static StoreItem Create(string code, string data, Guid userId) =>
        new() { Id = Guid.NewGuid(), Code = code, UserId = userId, Data = data };
}
