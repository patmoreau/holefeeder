using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.API.Authorization;
using DrifterApps.Holefeeder.Budgeting.Application;
using DrifterApps.Holefeeder.Budgeting.Application.Cashflows.Queries;
using DrifterApps.Holefeeder.Budgeting.Application.Models;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DrifterApps.Holefeeder.Budgeting.API.Controllers;

public static class CashflowsRoutes
{
    public static WebApplication AddCashflowsRoutes(this WebApplication app)
    {
        const string routePrefix = "api/v2/cashflows";

        app.MapGet($"{routePrefix}/get-upcoming",GetUpcoming)
            .WithName(nameof(GetUpcoming))
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces<UpcomingViewModel[]>()
            .AddOptions();

        app.MapGet($"{routePrefix}", GetCashflows)
            .WithName(nameof(GetCashflows))
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces<CashflowViewModel[]>()
            .AddOptions();

        app.MapGet($"{routePrefix}/{{id}}", GetCashflow)
            .WithName(nameof(GetCashflow))
            .Produces<CashflowViewModel>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddOptions();

        return app;
    }

    private static async Task<IResult> GetUpcoming(GetUpcoming.Request query, IMediator mediator, HttpResponse response)
    {
        var requestResult = await mediator.Send(query);
        return requestResult.Match(
            result => Results.ValidationProblem(
                result.Errors,
                statusCode: StatusCodes.Status422UnprocessableEntity,
                type: "https://httpstatuses.com/422"
            ),
            result =>
            {
                response.Headers.Add("X-Total-Count", $"{result.Total}");
                return Results.Ok(result.Items);
            });
    }

    private static async Task<IResult> GetCashflows(GetCashflows.Request query, IMediator mediator,
        HttpResponse response)
    {
        var requestResult = await mediator.Send(query);
        return requestResult.Match(
            result => Results.ValidationProblem(
                result.Errors,
                statusCode: StatusCodes.Status422UnprocessableEntity,
                type: "https://httpstatuses.com/422"
            ),
            result =>
            {
                response.Headers.Add("X-Total-Count", $"{result.Total}");
                return Results.Ok(result.Items);
            });
    }
    
    private static async Task<IResult> GetCashflow(Guid id, IMediator mediator, CancellationToken cancellationToken)
    {
        var requestResult = await mediator.Send(new GetCashflow.Request(id), cancellationToken);
        return requestResult.Match(
            Results.Ok,
            _ => Results.NotFound());
    }

    private static RouteHandlerBuilder AddOptions(this RouteHandlerBuilder builder) =>
        builder
            .WithTags("Cashflows")
            .RequireAuthorization()
            .WithGroupName("v2");
}
