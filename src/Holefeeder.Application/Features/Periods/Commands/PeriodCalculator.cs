using System.Text.Json;

using Holefeeder.Application.Context;
using Holefeeder.Application.Converters;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Domain.ValueObjects;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Periods.Commands;

public class PeriodCalculator : ICarterModule
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new DateOnlyJsonConverter()
        }
    };

    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/periods",
                async ([FromQuery] DateOnly asOfDate, [FromQuery] int? iteration, IUserContext userContext,
                    BudgetingContext context,
                    CancellationToken cancellationToken) =>
                {
                    var settings = await context.StoreItems
                        .Where(e => e.UserId == userContext.Id && e.Code == StoreItem.CodeSettings)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (settings is null)
                    {
                        return Results.NoContent();
                    }

                    var userSettings = JsonSerializer.Deserialize<UserSettings>(settings.Data, Options);

                    if (userSettings is null)
                    {
                        return Results.NoContent();
                    }

                    var startDate =
                        userSettings.IntervalType.AddIteration(asOfDate, (iteration ?? 0) * userSettings.Frequency);

                    var interval = userSettings.IntervalType.Interval(userSettings.EffectiveDate, startDate,
                        userSettings.Frequency);

                    return Results.Ok(new DateInterval(interval.from, interval.to));
                })
            .Produces<DateInterval>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(nameof(Periods))
            .RequireAuthorization(Policies.ReadUser);
}
