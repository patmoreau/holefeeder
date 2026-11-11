using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Transactions.Core.RecordTransfer;

internal sealed record RecordTransferRequest(User User, DateOnly Date, Money Amount, string Description, Id FromAccountId, Id ToAccountId);
