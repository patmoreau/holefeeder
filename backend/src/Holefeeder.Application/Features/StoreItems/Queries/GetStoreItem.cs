using Holefeeder.Domain.Features.StoreItem;

namespace Holefeeder.Application.Features.StoreItems.Queries;

public static class GetStoreItem
{
    public record Request(StoreItemId Id);
}
