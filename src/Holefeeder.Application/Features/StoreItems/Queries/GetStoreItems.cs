using System.Reflection;

using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Application.Extensions;

using Holefeeder.Application.Authorization;
using Holefeeder.Application.Context;
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
                    var results = await mediator.Send(request, cancellationToken);
                    ctx.Response.Headers.Append("X-Total-Count", $"{results.Total}");
                    return Results.Ok(results.Items);
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
        : IRequest<QueryResult<Response>>, IRequestQuery
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter) =>
            context.ToQueryRequest((offset, limit, sort, filter) => new Request(offset, limit, sort, filter));
    }

    internal record Response(Guid Id, string Code, string Data);

    internal class Validator : QueryValidatorRoot<Request>;

    internal class Handler(IUserContext userContext, BudgetingContext context) : IRequestHandler<Request, QueryResult<Response>>
    {
        private readonly BudgetingContext _context = context;
        private readonly IUserContext _userContext = userContext;

        public async Task<QueryResult<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var total = await _context.StoreItems.Where(e => e.UserId == _userContext.Id)
                .CountAsync(cancellationToken);
            var items = await _context.StoreItems
                .Where(e => e.UserId == _userContext.Id)
                .Filter(request.Filter)
                .Sort(request.Sort)
                .Skip(request.Offset)
                .Take(request.Limit)
                .Select(e => new Response(e.Id, e.Code, e.Data))
                .ToListAsync(cancellationToken);

            return new QueryResult<Response>(total, items);
        }
    }
}
