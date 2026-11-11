using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Dashboard.Core.GetUpcomingFlows;

internal sealed record UpcomingCashflow(
    Id Id,
    Cashflow Schedule,
    string Description,
    string CategoryName,
    string CategoryColor,
    IReadOnlyList<string> Tags);
