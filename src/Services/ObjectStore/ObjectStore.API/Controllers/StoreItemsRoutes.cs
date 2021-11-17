using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.ObjectStore.Application.StoreItems;
using DrifterApps.Holefeeder.ObjectStore.Application.StoreItems.Commands;
using DrifterApps.Holefeeder.ObjectStore.Application.StoreItems.Queries;

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

        app.MapGet($"{routePrefix}", GetStoreItems)
            .WithName(nameof(GetStoreItems))
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces<StoreItemViewModel[]>()
            .AddOptions();

        app.MapGet($"{routePrefix}/{{id}}", GetStoreItem)
            .WithName(nameof(GetStoreItem))
            .Produces<StoreItemViewModel>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddOptions();

        app.MapPost($"{routePrefix}/create-store-item", CreateStoreItem)
            .WithName(nameof(CreateStoreItem))
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .AddOptions();

        app.MapPost($"{routePrefix}/modify-store-item", ModifyStoreItem)
            .WithName(nameof(ModifyStoreItem))
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddOptions();

        return app;
    }

    private static async Task<IResult> GetStoreItems(GetStoreItems.Request request, IMediator mediator,
        HttpResponse response, CancellationToken cancellationToken)
    {
        var requestResult = await mediator.Send(request, cancellationToken);
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

    private static async Task<IResult> GetStoreItem(Guid id, IMediator mediator, CancellationToken cancellationToken)
    {
        var requestResult = await mediator.Send(new GetStoreItem.Request { Id = id }, cancellationToken);
        return requestResult.Match(
            Results.Ok,
            _ => Results.NotFound());
    }

    private static async Task<IResult> CreateStoreItem(CreateStoreItem.Request request, IMediator mediator,
        CancellationToken cancellationToken)
    {
        var requestResult = await mediator.Send(request, cancellationToken);
        return requestResult.Match(
            result => Results.ValidationProblem(
                result.Errors,
                statusCode: StatusCodes.Status422UnprocessableEntity,
                type: "https://httpstatuses.com/422"),
            result => Results.CreatedAtRoute(nameof(GetStoreItem), new { Id = result }, new { Id = result }),
            error => Results.BadRequest(new { error.Context, error.Message })
        );
    }

    private static async Task<IResult> ModifyStoreItem(ModifyStoreItem.Request request, IMediator mediator,
        CancellationToken cancellationToken)
    {
        var requestResult = await mediator.Send(request, cancellationToken);
        return requestResult.Match(
            result => Results.ValidationProblem(
                result.Errors,
                statusCode: StatusCodes.Status422UnprocessableEntity,
                type: "https://httpstatuses.com/422"),
            _ => Results.NotFound(),
            _ => Results.NoContent(),
            error => Results.BadRequest(new { error.Context, error.Message })
        );
    }

    private static void AddOptions(this RouteHandlerBuilder builder)
    {
        builder
            .WithTags("Store Items")
            .RequireAuthorization()
            .WithGroupName("v2");
    }
}
