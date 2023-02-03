using Holefeeder.Application.Exceptions;
using Holefeeder.Domain.Features.StoreItem;

namespace Holefeeder.Application.Features.StoreItems.Exceptions;

public class StoreItemNotFoundException : NotFoundException<StoreItem>
{
    public StoreItemNotFoundException(Guid id) : base(id)
    {
    }

    public StoreItemNotFoundException()
    {
    }

    public StoreItemNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public StoreItemNotFoundException(string message) : base(message)
    {
    }
}
