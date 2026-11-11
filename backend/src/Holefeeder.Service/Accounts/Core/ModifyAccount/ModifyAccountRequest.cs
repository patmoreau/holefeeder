using Holefeeder.Service.Shared.Core;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Accounts.Core.ModifyAccount;

internal sealed record ModifyAccountRequest(User User, Id AccountId, AccountName Name, Money? OpenBalance, string? Description);
