using Holefeeder.Service.Shared.Core;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.StoreItems.Core.ModifyStoreItem;

internal record ModifyStoreItemRequest(User User, Id Id, Data Data);
