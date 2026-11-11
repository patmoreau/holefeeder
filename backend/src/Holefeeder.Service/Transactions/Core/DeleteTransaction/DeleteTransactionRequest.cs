using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Transactions.Core.DeleteTransaction;

internal sealed record DeleteTransactionRequest(User User, Id Id);
