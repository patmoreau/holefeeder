using Holefeeder.Application.Exceptions;
using Holefeeder.Domain.Features.StoreItem;

namespace Holefeeder.Application.Features.StoreItems.Exceptions;

#pragma warning disable CA1032
public class StoreItemNotFoundException(Guid id) : NotFoundException<StoreItem>(id)
#pragma warning restore CA1032
{
}
