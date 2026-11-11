namespace Holefeeder.Service.Cashflows.Api.PayCashflow;

internal sealed record PayCashflowBody(string? CashflowId, string? Date, string? Amount, string? CashflowDate, string? UpdateRecurringAmount);
