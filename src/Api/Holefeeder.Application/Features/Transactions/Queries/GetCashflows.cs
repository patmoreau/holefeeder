using System.Reflection;

using Holefeeder.Application.Extensions;
using Holefeeder.Application.Models;
using Holefeeder.Application.SeedWork;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Transactions.Queries;

public class GetCashflows : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/cashflows",
                async (Request request, IMediator mediator, HttpContext ctx, CancellationToken cancellationToken) =>
                {
                    var (total, viewModels) = await mediator.Send(request, cancellationToken);
                    ctx.Response.Headers.Add("X-Total-Count", $"{total}");
                    return Results.Ok(viewModels);
                })
            .Produces<IEnumerable<CashflowInfoViewModel>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithMetadata(nameof(IRequestQuery))
            .WithTags(nameof(Transactions))
            .WithName(nameof(GetCashflows))
            .RequireAuthorization();
    }

    internal record Request(int Offset, int Limit, string[] Sort, string[] Filter)
        : IRequest<QueryResult<CashflowInfoViewModel>>, IRequestQuery
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            return context.ToQueryRequest((offset, limit, sort, filter) => new Request(offset, limit, sort, filter));
        }
    }

    internal class Validator : QueryValidatorRoot<Request>
    {
    }

    internal class Handler : IRequestHandler<Request, QueryResult<CashflowInfoViewModel>>
    {
        private readonly ICashflowQueriesRepository _repository;
        private readonly IUserContext _userContext;

        public Handler(IUserContext userContext, ICashflowQueriesRepository repository)
        {
            _userContext = userContext;
            _repository = repository;
        }

        public async Task<QueryResult<CashflowInfoViewModel>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            var (total, items) =
                await _repository.FindAsync(_userContext.UserId, QueryParams.Create(request), cancellationToken);

            return new QueryResult<CashflowInfoViewModel>(total, items);
        }
    }
}
