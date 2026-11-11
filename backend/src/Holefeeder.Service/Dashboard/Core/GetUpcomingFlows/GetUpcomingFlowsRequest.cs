using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Dashboard.Core.GetUpcomingFlows;

internal record GetUpcomingFlowsRequest(User User, DateOnly Today);
