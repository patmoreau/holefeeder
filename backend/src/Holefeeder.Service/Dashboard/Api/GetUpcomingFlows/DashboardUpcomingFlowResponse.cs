namespace Holefeeder.Service.Dashboard.Api.GetUpcomingFlows;

internal sealed record DashboardUpcomingFlowResponse(
    Guid CashflowId,
    DateOnly Date,
    string Title,
    string? Category,
    string CategoryColor,
    IReadOnlyList<string> Tags,
    decimal Amount);
