using Holefeeder.Service.Shared.Core;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Accounts.Core.FavoriteAccount;

internal sealed record FavoriteAccountRequest(User User, Id AccountId, bool IsFavorite);
