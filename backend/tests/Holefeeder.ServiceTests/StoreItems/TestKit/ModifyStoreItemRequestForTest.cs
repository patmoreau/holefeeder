using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.StoreItems.Core;
using Holefeeder.Service.StoreItems.Core.ModifyStoreItem;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.ServiceTests.StoreItems.TestKit;

internal static class ModifyStoreItemRequestForTest
{
    public static ModifyStoreItemRequest AModifyStoreItemRequest(User? user = null, Id? id = null, Data? data = null) =>
        new(user ?? AUser(), id ?? AnId(), data ?? Data.Valid("modified-data-value"));
}
