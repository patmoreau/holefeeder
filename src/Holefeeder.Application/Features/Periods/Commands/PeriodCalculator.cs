using Holefeeder.Application.Context;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.ValueObjects;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Periods.Commands;

public class PeriodCalculator : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/periods",
                ([FromQuery] DateOnly asOfDate, [FromQuery] int? iteration, IUserContext userContext,
                    BudgetingContext context,
                    CancellationToken cancellationToken) =>
                {
                    var userSettings = userContext.Settings;

                    var dateInterval = DateInterval.Create(asOfDate, iteration ?? 0, userSettings.EffectiveDate, userSettings.IntervalType, userSettings.Frequency);

                    return Results.Ok(dateInterval);
                })
            .Produces<DateInterval>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(nameof(Periods))
            .RequireAuthorization(Policies.ReadUser);
}
