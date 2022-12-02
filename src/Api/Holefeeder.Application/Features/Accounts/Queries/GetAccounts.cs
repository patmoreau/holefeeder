using System.Reflection;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.SeedWork;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Accounts.Queries;

public class GetAccounts : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/accounts",
                async (Request request, IMediator mediator, HttpContext ctx, CancellationToken cancellationToken) =>
                {
                    var (total, accountViewModels) = await mediator.Send(request, cancellationToken);
                    ctx.Response.Headers.Add("X-Total-Count", $"{total}");
                    return Results.Ok(accountViewModels);
                })
            .Produces<IEnumerable<AccountViewModel>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithMetadata(nameof(IRequestQuery))
            .WithTags(nameof(Accounts))
            .WithName(nameof(GetAccounts))
            .RequireAuthorization();
    }

    internal record Request(int Offset, int Limit, string[] Sort, string[] Filter)
        : IRequest<QueryResult<AccountViewModel>>, IRequestQuery
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            return context.ToQueryRequest((offset, limit, sort, filter) => new Request(offset, limit, sort, filter));
        }
    }

    internal class Validator : QueryValidatorRoot<Request>
    {
    }

    internal class Handler : IRequestHandler<Request, QueryResult<AccountViewModel>>
    {
        private readonly IAccountQueriesRepository _repository;
        private readonly IUserContext _userContext;
        private readonly BudgetingContext _context;

        public Handler(IUserContext userContext, BudgetingContext context, IAccountQueriesRepository repository)
        {
            _userContext = userContext;
            _context = context;
            _repository = repository;
        }

        public async Task<QueryResult<AccountViewModel>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            var (total, items) =
                await _repository.FindAsync(_userContext.UserId, QueryParams.Create(request), cancellationToken);

            return new QueryResult<AccountViewModel>(total, items);
        }
    }
}
