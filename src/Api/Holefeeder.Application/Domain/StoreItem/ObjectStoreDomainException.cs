using Holefeeder.Domain.SeedWork;

namespace Holefeeder.Application.Domain.StoreItem;

public class ObjectStoreDomainException : DomainException
{
    public ObjectStoreDomainException(string message) : base(message)
    {
    }

    public ObjectStoreDomainException()
    {
    }

    public ObjectStoreDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
