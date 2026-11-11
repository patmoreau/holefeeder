using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Transactions.Core.RecordTransfer;

internal sealed record RecordedTransfer(Id FromTransactionId, Id ToTransactionId);
