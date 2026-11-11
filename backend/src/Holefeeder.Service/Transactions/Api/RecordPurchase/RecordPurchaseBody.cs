namespace Holefeeder.Service.Transactions.Api.RecordPurchase;

internal sealed record RecordPurchaseBody(
    string? Date,
    string? Amount,
    string? Description,
    string? AccountId,
    string? CategoryId,
    string?[]? Tags,
    RecordPurchaseRepeatBody? Repeat);

internal sealed record RecordPurchaseRepeatBody(string? StartDate, string? IntervalType, string? Frequency);
