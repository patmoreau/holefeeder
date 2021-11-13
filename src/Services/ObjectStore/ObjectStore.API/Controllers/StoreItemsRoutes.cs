using System;
using System.Security.Claims;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.ObjectStore.API.Authorization;
using DrifterApps.Holefeeder.ObjectStore.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Commands;
using DrifterApps.Holefeeder.ObjectStore.Application.Queries;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DrifterApps.Holefeeder.ObjectStore.API.Controllers;

public static class StoreItemsRoutes
{
    public static WebApplication AddStoreItemsRoutes(this WebApplication app)
    {
        const string routePrefix = "api/v2/store-items";

        app.MapGet($"{routePrefix}",
                async (GetStoreItemsQuery query, IMediator mediator, ItemsCache cache, ClaimsPrincipal user, HttpResponse response) =>
                {
                    cache.Add("UserId", user.GetUniqueId());
                    var result = await mediator.Send(query);
                    response.Headers.Add("X-Total-Count", $"{result.Total}");
                    return Results.Ok(result.Items);
                })
            .WithTags("Store Items")
            .AddOptions();

        app.MapGet($"{routePrefix}/{{id}}",
                async (Guid id, IMediator mediator, ItemsCache cache, ClaimsPrincipal user) =>
                {
                    cache.Add("UserId", user.GetUniqueId());
                    var result = await mediator.Send(new GetStoreItemQuery(id));
                    return result is null ? Results.NotFound() : Results.Ok(result);
                })
            .WithTags("Store Items")
            .AddOptions();

        app.MapPost($"{routePrefix}/create-store-item",
                async ([FromBody]CreateStoreItemCommand command, IMediator mediator, ItemsCache cache, ClaimsPrincipal user) =>
                {
                    cache.Add("UserId", user.GetUniqueId());
                    var result = await mediator.Send(command);
                    return Results.Ok(result);
                })
            .WithTags("Store Items")
            .AddOptions();

        app.MapPost($"{routePrefix}/modify-store-item",
            async (ModifyStoreItemCommand command, IMediator mediator, ItemsCache cache, ClaimsPrincipal user) =>
            {
                cache.Add("UserId", user.GetUniqueId());
                var result = await mediator.Send(command);
                return result ? Results.NoContent() : Results.NotFound();
                
            })
            .WithTags("Store Items")
            .AddOptions();

        return app;
    }

    private static IEndpointConventionBuilder AddOptions(this IEndpointConventionBuilder builder) =>
        builder
            .RequireAuthorization() //(Scopes.REGISTERED_USER)
            .WithGroupName("v2");
}
