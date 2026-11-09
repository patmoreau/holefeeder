using Holefeeder.Service.Shared.Core;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.StoreItems.Core.GetStoreItem;

internal record GetStoreItemRequest(User User, Id Id);
