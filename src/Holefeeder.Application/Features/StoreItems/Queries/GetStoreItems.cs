using System.Reflection;

using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Application.Extensions;
using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.UserContext;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.StoreItems.Queries;

public class GetStoreItems : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/store-items",
                async (Request request, IMediator mediator, HttpContext ctx, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(request, cancellationToken);
                    switch (result)
                    {
                        case { IsFailure: true }:
                            return result.Error.ToProblem();
                        default:
                            ctx.Response.Headers.Append("X-Total-Count", $"{result.Value.Total}");
                            return Results.Ok(result.Value.Items);
                    }

                })
            .Produces<QueryResult<Response>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithMetadata(nameof(IRequestQuery))
            .WithTags(nameof(StoreItems))
            .WithName(nameof(GetStoreItems))
            .RequireAuthorization(Policies.ReadUser);

    internal record Request(int Offset, int Limit, string[] Sort, string[] Filter)
        : IRequest<Result<QueryResult<Response>>>, IRequestQuery
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter) =>
            context.ToQueryRequest((offset, limit, sort, filter) => new Request(offset, limit, sort, filter));
    }

    internal record Response(Guid Id, string Code, string Data);

    internal class Validator : QueryValidatorRoot<Request>;

    internal class Handler(IUserContext userContext, BudgetingContext context) : IRequestHandler<Request, Result<QueryResult<Response>>>
    {
        public async Task<Result<QueryResult<Response>>> Handle(Request request, CancellationToken cancellationToken)
        {
            var queryParams = QueryParams.Create(request);
            if (queryParams.IsFailure)
            {
                return queryParams.Error;
            }
            var total = await context.StoreItems.Where(e => e.UserId == userContext.Id)
                .CountAsync(cancellationToken);
            var items = await context.StoreItems
                .Where(e => e.UserId == userContext.Id)
                .Query(queryParams.Value)
                .Select(e => new Response(e.Id, e.Code, e.Data))
                .ToListAsync(cancellationToken);

            return new QueryResult<Response>(total, items);
        }
    }
}
