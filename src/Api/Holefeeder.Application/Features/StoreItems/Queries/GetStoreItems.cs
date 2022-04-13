using System.Reflection;

using Carter;

using Holefeeder.Application.Extensions;
using Holefeeder.Application.SeedWork;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.StoreItems.Queries;

public class GetStoreItems : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/store-items",
                async (Request request, IMediator mediator, HttpContext ctx, CancellationToken cancellationToken) =>
                {
                    var results = await mediator.Send(request, cancellationToken);
                    ctx.Response.Headers.Add("X-Total-Count", $"{results.Total}");
                    return Results.Ok(results.Items);
                })
            .Produces<IEnumerable<StoreItemViewModel>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithMetadata(nameof(IRequestQuery))
            .WithTags(nameof(StoreItems))
            .WithName(nameof(GetStoreItems))
            .RequireAuthorization();
    }

    public record Request(int Offset, int Limit, string[] Sort, string[] Filter)
        : IRequest<QueryResult<StoreItemViewModel>>, IRequestQuery
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            return context.ToQueryRequest((offset, limit, sort, filter) => new Request(offset, limit, sort, filter));
        }
    }

    public class Validator : QueryValidatorRoot<Request>
    {
    }

    public class Handler : IRequestHandler<Request, QueryResult<StoreItemViewModel>>
    {
        private readonly IStoreItemsQueriesRepository _repository;
        private readonly IUserContext _userContext;

        public Handler(IUserContext userContext, IStoreItemsQueriesRepository repository)
        {
            _userContext = userContext;
            _repository = repository;
        }

        public async Task<QueryResult<StoreItemViewModel>> Handle(Request request, CancellationToken cancellationToken)
        {
            var (total, items) =
                await _repository.FindAsync(_userContext.UserId, QueryParams.Create(request), cancellationToken);

            return new QueryResult<StoreItemViewModel>(total, items);
        }
    }
}
