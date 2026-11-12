using Holefeeder.Domain.Features.StoreItem;

namespace Holefeeder.Application.Features.StoreItems.Commands;

public static class ModifyStoreItem
{
    public record Request(StoreItemId Id, string Data);
}
