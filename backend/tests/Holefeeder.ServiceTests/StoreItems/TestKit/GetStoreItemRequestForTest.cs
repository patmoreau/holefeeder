using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.StoreItems.Core.GetStoreItem;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.ServiceTests.StoreItems.TestKit;

internal static class GetStoreItemRequestForTest
{
    public static GetStoreItemRequest AGetStoreItemRequest(User? user = null, Id? id = null) =>
        new(user ?? AUser(), id ?? AnId());
}
