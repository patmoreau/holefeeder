using Holefeeder.Service.Shared.Core;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Accounts.Core.CloseAccount;

internal sealed record CloseAccountRequest(User User, Id AccountId);
