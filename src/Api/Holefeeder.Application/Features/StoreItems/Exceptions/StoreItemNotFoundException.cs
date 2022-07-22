using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Domain.SeedWork;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.Application.Features.StoreItems.Exceptions;

public class StoreItemNotFoundException : DomainException
{
    public StoreItemNotFoundException(Guid id) : base(StatusCodes.Status404NotFound,
        $"{nameof(StoreItem)} '{id}' not found")
    {
    }

    public override string Context => nameof(StoreItems);
}
