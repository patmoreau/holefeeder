using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands;
using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Queries;
using DrifterApps.Holefeeder.Budgeting.Application.Models;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace DrifterApps.Holefeeder.Budgeting.API.Controllers;

public static class AccountsRoutes
{
    public static WebApplication AddAccountsRoutes(this WebApplication app)
    {
        const string routePrefix = "api/v2/accounts";

        app.MapGet($"{routePrefix}", GetAccounts)
            .WithName(nameof(GetAccounts))
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces<AccountViewModel[]>()
            .AddOptions();

        app.MapGet($"{routePrefix}/{{id}}", GetAccount)
            .WithName(nameof(GetAccount))
            .Produces<AccountViewModel>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddOptions();

        app.MapPost($"{routePrefix}/open-account", OpenAccount)
            .WithName(nameof(OpenAccount))
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .AddOptions();

        app.MapPost($"{routePrefix}/modify-account", ModifyAccount)
            .WithName(nameof(ModifyAccount))
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddOptions();

        app.MapPost($"{routePrefix}/close-account", CloseAccount)
            .WithName(nameof(CloseAccount))
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddOptions();

        app.MapPost($"{routePrefix}/favorite-account", FavoriteAccount)
            .WithName(nameof(FavoriteAccount))
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddOptions();

        return app;
    }

    private static async Task<IResult> GetAccounts(GetAccounts.Request query, IMediator mediator, HttpResponse response)
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

    private static async Task<IResult> GetAccount(Guid id, IMediator mediator, CancellationToken cancellationToken)
    {
        var requestResult = await mediator.Send(new GetAccount.Request(id), cancellationToken);
        return requestResult.Match(
            Results.Ok,
            _ => Results.NotFound());
    }

    private static async Task<IResult> OpenAccount(OpenAccount.Request request, IMediator mediator,
        CancellationToken cancellationToken)
    {
        var requestResult = await mediator.Send(request, cancellationToken);
        return requestResult.Match(
            result => Results.ValidationProblem(
                result.Errors,
                statusCode: StatusCodes.Status422UnprocessableEntity,
                type: "https://httpstatuses.com/422"),
            result => Results.CreatedAtRoute(nameof(GetAccount), new { Id = result }, new { Id = result }),
            error => Results.BadRequest(new { error.Context, error.Message })
        );
    }

    private static async Task<IResult> ModifyAccount(ModifyAccount.Request request, IMediator mediator,
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

    private static async Task<IResult> CloseAccount(CloseAccount.Request request, IMediator mediator,
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
            error => Results.Problem(error.Message, statusCode: StatusCodes.Status400BadRequest,
                title: error.Context)
        );
    }

    private static async Task<IResult> FavoriteAccount(FavoriteAccount.Request request, IMediator mediator,
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
            .WithTags("Accounts")
            .RequireAuthorization()
            .WithGroupName("v2");
    }
}
