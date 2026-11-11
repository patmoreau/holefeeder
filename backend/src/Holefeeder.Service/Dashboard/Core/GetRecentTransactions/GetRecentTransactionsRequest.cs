using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Dashboard.Core.GetRecentTransactions;

internal record GetRecentTransactionsRequest(User User, int Count);
