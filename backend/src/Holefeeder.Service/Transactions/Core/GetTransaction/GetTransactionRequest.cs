using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Transactions.Core.GetTransaction;

internal sealed record GetTransactionRequest(User User, Id Id);
