using Holefeeder.Domain.SeedWork;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.Domain.Features.StoreItem;

public class ObjectStoreDomainException : DomainException
{
    public ObjectStoreDomainException(string message) : base(StatusCodes.Status400BadRequest, message)
    {
    }

    public ObjectStoreDomainException()
    {
    }

    public ObjectStoreDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override string Context => nameof(StoreItem);
}
