namespace Holefeeder.Service.Transactions.Api.RecordTransfer;

internal sealed record RecordedTransferResponse(Guid FromTransactionId, Guid ToTransactionId);
