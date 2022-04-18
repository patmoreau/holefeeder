using System.Reflection;

using Carter;

using DrifterApps.Holefeeder.Budgeting.Application.Models;

using Holefeeder.Application.Extensions;
using Holefeeder.Application.SeedWork;

using MediatR;

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

    public record Request(int Offset, int Limit, string[] Sort, string[] Filter)
        : IRequest<QueryResult<AccountViewModel>>, IRequestQuery
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            return context.ToQueryRequest((offset, limit, sort, filter) => new Request(offset, limit, sort, filter));
        }
    }

    public class Validator : QueryValidatorRoot<Request>
    {
    }

    public class Handler : IRequestHandler<Request, QueryResult<AccountViewModel>>
    {
        private readonly IUserContext _userContext;
        private readonly IAccountQueriesRepository _repository;

        public Handler(IUserContext userContext, IAccountQueriesRepository repository)
        {
            _userContext = userContext;
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
