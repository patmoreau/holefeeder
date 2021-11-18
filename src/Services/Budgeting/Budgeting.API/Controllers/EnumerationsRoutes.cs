using DrifterApps.Holefeeder.Budgeting.Application.Enumerations.Queries;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace DrifterApps.Holefeeder.Budgeting.API.Controllers;

public static class EnumerationsRoutes
{
    public static WebApplication AddEnumerationsRoutes(this WebApplication app)
    {
        const string routePrefix = "api/v2/enumerations";

        app.MapGet($"{routePrefix}/get-account-types",
                async (IMediator mediator) =>
                {
                    var result = await mediator.Send(new GetAccountTypes.Request());
                    return Results.Ok(result);
                })
            .Produces<AccountType[]>()
            .AddOptions();

        app.MapGet($"{routePrefix}/get-category-types",
                async (IMediator mediator) =>
                {
                    var result = await mediator.Send(new GetCategoryTypes.Request());
                    return Results.Ok(result);
                })
            .Produces<CategoryType[]>()
            .AddOptions();

        app.MapGet($"{routePrefix}/get-date-interval-types",
                async (IMediator mediator) =>
                {
                    var result = await mediator.Send(new GetDateIntervalTypes.Request());
                    return Results.Ok(result);
                })
            .Produces<DateIntervalType[]>()
            .AddOptions();

        return app;
    }

    private static void AddOptions(this RouteHandlerBuilder builder)
    {
        builder
            .WithTags("Enumerations")
            .RequireAuthorization()
            .WithGroupName("v2");
    }
}
