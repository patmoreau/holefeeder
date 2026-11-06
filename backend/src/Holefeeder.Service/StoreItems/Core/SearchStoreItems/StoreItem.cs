using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.StoreItems.Core.SearchStoreItems;

internal sealed record StoreItem(Id Id, Code Code, string Data)
{
    public static Result<StoreItem> Create(Id id, Code code, string data) => new StoreItem(id, code, data);

    public static StoreItem Valid(Id id, Code code, string data) => new(id, code, data);
}
