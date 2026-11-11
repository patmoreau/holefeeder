using Holefeeder.Service.Shared.Core;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Accounts.Core.GetAccountDetail;

internal record GetAccountDetailRequest(User User, Id AccountId, DateOnly Today);
