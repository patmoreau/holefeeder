using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Cashflows.Core.ListCashflows;

internal sealed record CashflowSummary(
    Id Id,
    string Description,
    Id CategoryId,
    CashflowSchedule Schedule,
    string CategoryName,
    string CategoryColor,
    string AccountName,
    IReadOnlyList<string> Tags)
{
    public FlowTitle Title => FlowTitle.Of(Description, CategoryName);
}
