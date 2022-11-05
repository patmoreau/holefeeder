using Holefeeder.Domain.SeedWork;

namespace Holefeeder.Application.Domain.StoreItem;

#pragma warning disable CA1032
public class ObjectStoreDomainException : DomainException<Holefeeder.Domain.Features.StoreItem.StoreItem>
#pragma warning restore CA1032
{
    public ObjectStoreDomainException(string message) : base(message)
    {
    }

    public ObjectStoreDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
