namespace Holefeeder.Domain.Features.StoreItem;

#pragma warning disable CA1032
public class ObjectStoreDomainException(string message) : DomainException<StoreItem>(message)
#pragma warning restore CA1032
{
}
