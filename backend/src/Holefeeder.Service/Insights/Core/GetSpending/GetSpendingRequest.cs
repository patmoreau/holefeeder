using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Insights.Core.GetSpending;

internal sealed record GetSpendingRequest(User User, DateOnly Today);
