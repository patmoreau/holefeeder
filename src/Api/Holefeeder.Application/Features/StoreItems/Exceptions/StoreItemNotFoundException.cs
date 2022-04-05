using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Domain.SeedWork;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.Application.Features.StoreItems.Exceptions;

public class StoreItemNotFoundException : DomainException
{
    public StoreItemNotFoundException(Guid id) : base(StatusCodes.Status400BadRequest,
        $"{nameof(StoreItem)} '{id}' not found")
    {
    }

    public override string Context => nameof(StoreItemNotFoundException);
}
