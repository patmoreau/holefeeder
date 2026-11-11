namespace Holefeeder.Service.Transactions.Api.RecordTransfer;

internal sealed record RecordTransferBody(string? Date, string? Amount, string? Description, string? FromAccountId, string? ToAccountId);
