using System;

using DrifterApps.Holefeeder.Framework.SeedWork.Domain;
using DrifterApps.Holefeeder.ObjectStore.Domain.Exceptions;

namespace DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext;

public record StoreItem : IAggregateRoot
{
    private readonly Guid _id;
    private readonly string _code = null!;
    private readonly Guid _userId;

    public Guid Id
    {
        get => _id;
        init
        {
            if (value.Equals(default))
            {
                throw new ObjectStoreDomainException(nameof(StoreItem), $"{nameof(Id)} is required");
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
                throw new ObjectStoreDomainException(nameof(StoreItem), $"{nameof(Code)} is required");
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
                throw new ObjectStoreDomainException(nameof(StoreItem), $"{nameof(UserId)} is required");
            }

            _userId = value;
        }
    }

    public static StoreItem Create(string code, string data, Guid userId) =>
        new() {Id = Guid.NewGuid(), Code = code, Data = data, UserId = userId};
}
