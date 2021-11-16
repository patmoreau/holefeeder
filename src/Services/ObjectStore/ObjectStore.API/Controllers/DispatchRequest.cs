using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace DrifterApps.Holefeeder.ObjectStore.API.Controllers;

public static class DispatchRequest
{
    public static async Task<IResult> ModifyAsync<TRequest>(TRequest request, IMediator mediator,
        CancellationToken cancellationToken)
        where TRequest : IRequest<IRequestResult>
    {
        var requestResult = await mediator.Send(request, cancellationToken);
        return requestResult switch
        {
            ValidationErrorsRequestResult result => Results.ValidationProblem(result.Errors,
                title: "Invalid Request", statusCode: StatusCodes.Status422UnprocessableEntity,
                type: "https://httpstatuses.com/422"),
            NotFoundRequestResult => Results.NotFound(),
            NoContentResult => Results.NoContent(),
            _ => throw new InvalidOperationException($"Unexpected request result: {requestResult.GetType()}")
        };
    }

    public static async Task<IResult> CreateAsync<TRequest>(TRequest request, IMediator mediator,
        CancellationToken cancellationToken)
        where TRequest : IRequest<IRequestResult>
    {
        var requestResult = await mediator.Send(request, cancellationToken);
        return requestResult switch
        {
            ValidationErrorsRequestResult result => Results.ValidationProblem(errors: result.Errors,
                title: "Invalid Request", statusCode: StatusCodes.Status422UnprocessableEntity,
                type: "https://httpstatuses.com/422"),
            CreatedRequestResult(var guid, var name) => Results.CreatedAtRoute(name, new { Id = guid },
                new { Id = guid }),
            _ => throw new InvalidOperationException($"Unexpected request result: {requestResult.GetType()}")
        };
    }

    public static async Task<IResult> WithIdAsync<TRequest>(Guid id, IMediator mediator,
        CancellationToken cancellationToken)
        where TRequest : IRequest<IRequestResult>, IRequestById, new()
    {
        var requestResult = await mediator.Send(new TRequest { Id = id }, cancellationToken);
        return requestResult switch
        {
            NotFoundRequestResult => Results.NotFound(),
            IdRequestResult result => Results.Ok(result.Item),
            _ => throw new InvalidOperationException($"Unexpected request result: {requestResult.GetType()}")
        };
    }

    public static async Task<IResult> QueryAsync<TRequest>(TRequest request, IMediator mediator,
        HttpResponse response, CancellationToken cancellationToken)
        where TRequest : IRequest<IRequestResult>, IRequestQuery
    {
        var requestResult = await mediator.Send(request, cancellationToken);
        switch (requestResult)
        {
            case ValidationErrorsRequestResult result:
                return Results.ValidationProblem(
                    errors: result.Errors,
                    title: "Invalid Request",
                    statusCode: StatusCodes.Status422UnprocessableEntity,
                    type: "https://httpstatuses.com/422"
                );
            case ListRequestResult(var total, var items):
                response.Headers.Add("X-Total-Count", $"{total}");
                return Results.Ok(items);
            default:
                throw new InvalidOperationException($"Unexpected request result: {requestResult.GetType()}");
        }
    }
}
