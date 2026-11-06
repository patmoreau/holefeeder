using Holefeeder.Service.StoreItems.Core.SearchStoreItems;

namespace Holefeeder.Service.StoreItems.Persistence;

internal sealed record StoreItemRow(Guid Id, string Code, string Data)
{
    public StoreItem ToStoreItem() => new(Shared.Core.Id.Valid(Id), Core.Code.Valid(Code), Data);
}
