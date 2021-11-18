using System;
using System.Security.Claims;

using DrifterApps.Holefeeder.Budgeting.API.Authorization;
using DrifterApps.Holefeeder.Budgeting.Application;
using DrifterApps.Holefeeder.Budgeting.Application.Cashflows.Queries;
using DrifterApps.Holefeeder.Budgeting.Application.Enumerations.Queries;
using DrifterApps.Holefeeder.Budgeting.Application.Imports.Commands;
using DrifterApps.Holefeeder.Budgeting.Application.Imports.Queries;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace DrifterApps.Holefeeder.Budgeting.API.Controllers;

public static class ImportsRoutes
{
    public static WebApplication AddImportsRoutes(this WebApplication app)
    {
        const string routePrefix = "api/v2/imports";

        app.MapPost($"{routePrefix}/import-data",
                async (ImportDataCommand command, IMediator mediator, ItemsCache cache, ClaimsPrincipal user) =>
                {
                    cache.Add("UserId", user.GetUniqueId());
                    var result = await mediator.Send(command);
                    return Results.Accepted(value: new { Id = result });
                })
            .AddOptions();

        app.MapGet($"{routePrefix}/{{id}}",
                async (Guid id, IMediator mediator, ItemsCache cache, ClaimsPrincipal user) =>
                {
                    cache.Add("UserId", user.GetUniqueId());
                    var result = await mediator.Send(new ImportDataStatusQuery(id));
                    return Results.Ok(result);
                })
            .AddOptions();

        return app;
    }

    private static RouteHandlerBuilder AddOptions(this RouteHandlerBuilder builder) =>
        builder
            .WithTags("Imports")
            .RequireAuthorization()
            .WithGroupName("v2");
}
