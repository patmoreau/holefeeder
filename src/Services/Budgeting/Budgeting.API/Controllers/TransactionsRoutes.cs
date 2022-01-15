using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Application.Transactions.Commands;
using DrifterApps.Holefeeder.Budgeting.Application.Transactions.Queries;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace DrifterApps.Holefeeder.Budgeting.API.Controllers;

public static class TransactionsRoutes
{
    public static WebApplication AddTransactionsRoutes(this WebApplication app)
    {
        const string routePrefix = "api/v2/transactions";

        app.MapGet($"{routePrefix}", GetTransactions)
            .WithName(nameof(GetTransactions))
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces<TransactionInfoViewModel[]>()
            .AddOptions();

        app.MapGet($"{routePrefix}/{{id}}", GetTransaction)
            .WithName(nameof(GetTransaction))
            .Produces<TransactionInfoViewModel>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddOptions();

        app.MapPost($"{routePrefix}/modify", ModifyTransaction)
            .WithName(nameof(ModifyTransaction))
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddOptions();

        app.MapPost($"{routePrefix}/make-purchase", MakePurchase)
            .WithName(nameof(MakePurchase))
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .AddOptions();

        app.MapPost($"{routePrefix}/transfer", Transfer)
            .WithName(nameof(Transfer))
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .AddOptions();

        app.MapPost($"{routePrefix}/pay-cashflow", PayCashflow)
            .WithName(nameof(PayCashflow))
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .AddOptions();

        app.MapDelete($"{routePrefix}/{{id}}", DeleteTransaction)
            .WithName(nameof(DeleteTransaction))
            .Produces<TransactionInfoViewModel>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddOptions();

        return app;
    }

    private static async Task<IResult> GetTransactions(GetTransactions.Request query, IMediator mediator, HttpResponse response)
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
    
    private static async Task<IResult> GetTransaction(Guid id, IMediator mediator, CancellationToken cancellationToken)
    {
        var requestResult = await mediator.Send(new GetTransaction.Request(id), cancellationToken);
        return requestResult.Match(
            Results.Ok,
            _ => Results.NotFound());
    }

    private static async Task<IResult> ModifyTransaction(ModifyTransaction.Request request, IMediator mediator,
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

    private static async Task<IResult> MakePurchase(MakePurchase.Request request, IMediator mediator,
        CancellationToken cancellationToken)
    {
        var requestResult = await mediator.Send(request, cancellationToken);
        return requestResult.Match(
            result => Results.ValidationProblem(
                result.Errors,
                statusCode: StatusCodes.Status422UnprocessableEntity,
                type: "https://httpstatuses.com/422"),
            result => Results.CreatedAtRoute(nameof(GetTransaction), new { Id = result }, new { Id = result }),
            error => Results.BadRequest(new { error.Context, error.Message })
        );
    }

    private static async Task<IResult> Transfer(Transfer.Request request, IMediator mediator,
        CancellationToken cancellationToken)
    {
        var requestResult = await mediator.Send(request, cancellationToken);
        return requestResult.Match(
            result => Results.ValidationProblem(
                result.Errors,
                statusCode: StatusCodes.Status422UnprocessableEntity,
                type: "https://httpstatuses.com/422"),
            result => Results.CreatedAtRoute(nameof(GetTransaction), new { Id = result }, new { Id = result }),
            error => Results.BadRequest(new { error.Context, error.Message })
        );
    }

    private static async Task<IResult> PayCashflow(PayCashflow.Request request, IMediator mediator,
        CancellationToken cancellationToken)
    {
        var requestResult = await mediator.Send(request, cancellationToken);
        return requestResult.Match(
            result => Results.ValidationProblem(
                result.Errors,
                statusCode: StatusCodes.Status422UnprocessableEntity,
                type: "https://httpstatuses.com/422"),
            result => Results.CreatedAtRoute(nameof(GetTransaction), new { Id = result }, new { Id = result }),
            error => Results.BadRequest(new { error.Context, error.Message })
        );
    }
    
    private static async Task<IResult> DeleteTransaction(Guid id, IMediator mediator, CancellationToken cancellationToken)
    {
        var requestResult = await mediator.Send(new DeleteTransaction.Request(id), cancellationToken);
        return requestResult.Match(
            result => Results.ValidationProblem(
                result.Errors,
                statusCode: StatusCodes.Status422UnprocessableEntity,
                type: "https://httpstatuses.com/422"),
            _ => Results.NoContent(),
            _ => Results.NotFound());
    }

    private static RouteHandlerBuilder AddOptions(this RouteHandlerBuilder builder) =>
        builder
            .WithTags("Transactions")
            .RequireAuthorization()
            .WithGroupName("v2");
}
