namespace Holefeeder.Service.Cashflows.Api.ListCashflows;

internal sealed record CashflowResponse(
    Guid Id,
    string Title,
    string Description,
    Guid CategoryId,
    string CategoryName,
    string CategoryColor,
    string CategoryType,
    string AccountName,
    decimal Amount,
    string IntervalType,
    int Frequency,
    DateOnly EffectiveDate,
    DateOnly? NextDate,
    IReadOnlyList<string> Tags);
