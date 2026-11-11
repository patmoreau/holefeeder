using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Shared.Core.Search;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Accounts.Core.GetAccountTransactions;

internal record GetAccountTransactionsRequest(User User, Id AccountId, Offset Offset, Limit Limit);
