using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.StoreItems.Core.CreateStoreItem;

internal record CreateStoreItemRequest(User User, Code Code, Data Data);
