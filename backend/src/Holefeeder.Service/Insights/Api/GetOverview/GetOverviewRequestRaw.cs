using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Insights.Core.GetOverview;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Insights.Api.GetOverview;

internal sealed record GetOverviewRequestRaw([FromQuery(Name = "periods")] string? PeriodsRaw)
{
    public static Result<GetOverviewRequest> ToGetOverviewRequest(GetOverviewRequestRaw raw, User user, DateOnly today)
    {
        var periods = PeriodCount.Create(raw.PeriodsRaw);
        return periods.IsFailure ? periods.Error : new GetOverviewRequest(user, today, periods.Value);
    }
}
