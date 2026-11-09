using Holefeeder.Service.StoreItems.Core;
using Holefeeder.Service.StoreItems.Core.CreateStoreItem;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.ServiceTests.StoreItems.TestKit;

internal static class CreateStoreItemRequestForTest
{
    public static CreateStoreItemRequest ACreateStoreItemRequest(User? user = null, Code? code = null, Data? data = null) =>
        new(user ?? AUser(), code ?? ACode(), data ?? Data.Valid("created-data-value"));
}
