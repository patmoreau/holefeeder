using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Dashboard.Core.GetUpcomingFlows;

internal sealed record DashboardUpcomingFlow(
    Id CashflowId,
    DateOnly Date,
    string Title,
    string? Category,
    string CategoryColor,
    IReadOnlyList<string> Tags,
    decimal Amount);
