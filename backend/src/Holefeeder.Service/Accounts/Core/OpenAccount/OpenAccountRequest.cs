using Holefeeder.Service.Shared.Core;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Accounts.Core.OpenAccount;

internal sealed record OpenAccountRequest(
    User User,
    AccountType Type,
    AccountName Name,
    DateOnly OpenDate,
    Money OpenBalance,
    string Description);
