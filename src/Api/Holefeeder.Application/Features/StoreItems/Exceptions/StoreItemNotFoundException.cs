using Holefeeder.Application.Domain.StoreItem;
using Holefeeder.Application.Exceptions;
using Holefeeder.Domain.Features.StoreItem;

namespace Holefeeder.Application.Features.StoreItems.Exceptions;

#pragma warning disable CA1032
public class StoreItemNotFoundException : NotFoundException<StoreItem>
#pragma warning restore CA1032
{
    public StoreItemNotFoundException(Guid id) : base(id)
    {
    }
}
