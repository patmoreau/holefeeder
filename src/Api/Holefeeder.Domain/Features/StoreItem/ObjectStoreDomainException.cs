using Holefeeder.Domain.SeedWork;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.Domain.Features.StoreItem;

public class ObjectStoreDomainException : DomainException
{
    public ObjectStoreDomainException(string message) : base(StatusCodes.Status400BadRequest, message)
    {
    }

    public override string Context => nameof(ObjectStoreDomainException);
}
