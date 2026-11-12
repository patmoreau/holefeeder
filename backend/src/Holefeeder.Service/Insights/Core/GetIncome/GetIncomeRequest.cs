using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Insights.Core.GetIncome;

internal sealed record GetIncomeRequest(User User, DateOnly Today);
