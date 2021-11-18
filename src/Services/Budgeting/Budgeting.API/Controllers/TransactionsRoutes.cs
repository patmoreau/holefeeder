using System;
using System.Security.Claims;

using DrifterApps.Holefeeder.Budgeting.API.Authorization;
using DrifterApps.Holefeeder.Budgeting.Application;
using DrifterApps.Holefeeder.Budgeting.Application.Transactions.Commands;
using DrifterApps.Holefeeder.Budgeting.Application.Transactions.Queries;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DrifterApps.Holefeeder.Budgeting.API.Controllers;

public static class TransactionsRoutes
{
    public static WebApplication AddTransactionsRoutes(this WebApplication app)
    {
        const string routePrefix = "api/v2/transactions";

        app.MapGet($"{routePrefix}",
                async (GetTransactionsRequestQuery query, IMediator mediator, ItemsCache cache, ClaimsPrincipal user, HttpResponse response) =>
                {
                    cache.Add("UserId", user.GetUniqueId());
                    var (total, items) = await mediator.Send(query);
                    response.Headers.Add("X-Total-Count", $"{total}");
                    return Results.Ok(items);
                })
            .AddOptions();

        app.MapGet($"{routePrefix}/{{id}}",
                async (Guid id, IMediator mediator, ItemsCache cache, ClaimsPrincipal user) =>
                {
                    cache.Add("UserId", user.GetUniqueId());
                    var result = await mediator.Send(new GetTransactionQuery(id));
                    return Results.Ok(result);
                })
            .AddOptions();

        app.MapPost($"{routePrefix}/make-purchase",
                async ([FromBody]MakePurchaseCommand command, IMediator mediator, ItemsCache cache, ClaimsPrincipal user) =>
                {
                    cache.Add("UserId", user.GetUniqueId());
                    var result = await mediator.Send(command);
                    return Results.Ok(result);
                })
            .AddOptions();

        app.MapPost($"{routePrefix}/transfer",
                async (TransferCommand command, IMediator mediator, ItemsCache cache, ClaimsPrincipal user) =>
                {
                    cache.Add("UserId", user.GetUniqueId());
                    await mediator.Send(command);
                    return Results.NoContent();
                
                })
            .AddOptions();

        return app;
    }

    private static RouteHandlerBuilder AddOptions(this RouteHandlerBuilder builder) =>
        builder
            .WithTags("Transactions")
            .RequireAuthorization()
            .WithGroupName("v2");
}
