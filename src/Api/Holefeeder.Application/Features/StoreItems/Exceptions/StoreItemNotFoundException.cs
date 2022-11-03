using Holefeeder.Application.Domain.StoreItem;
using Holefeeder.Application.Exceptions;

namespace Holefeeder.Application.Features.StoreItems.Exceptions;

#pragma warning disable CA1032
public class StoreItemNotFoundException : NotFoundException<StoreItem>
#pragma warning restore CA1032
{
    public StoreItemNotFoundException(Guid id) : base(id)
    {
    }
}
