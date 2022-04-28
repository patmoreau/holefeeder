using System.Reflection;

using Carter;

using FluentValidation.Results;

using Holefeeder.Application.Extensions;
using Holefeeder.Application.Features.Categories.Queries;
using Holefeeder.Application.Models;
using Holefeeder.Application.SeedWork;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Transactions.Queries;

public class GetTransactions : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/transactions",
                async (Request request, IMediator mediator, HttpContext ctx, CancellationToken cancellationToken) =>
                {
                    var (total, viewModels) = await mediator.Send(request, cancellationToken);
                    ctx.Response.Headers.Add("X-Total-Count", $"{total}");
                    return Results.Ok(viewModels);
                })
            .Produces<IEnumerable<TransactionInfoViewModel>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithMetadata(nameof(IRequestQuery))
            .WithTags(nameof(Transactions))
            .WithName(nameof(GetTransactions))
            .RequireAuthorization();
    }

    public record Request(int Offset, int Limit, string[] Sort, string[] Filter)
        : IRequest<QueryResult<TransactionInfoViewModel>>, IRequestQuery
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            return context.ToQueryRequest((offset, limit, sort, filter) => new Request(offset, limit, sort, filter));
        }
    }

    public class Validator : QueryValidatorRoot<Request>
    {
    }

    public class Handler : IRequestHandler<Request, QueryResult<TransactionInfoViewModel>>
    {
        private readonly IUserContext _userContext;
        private readonly ITransactionQueriesRepository _repository;

        public Handler(IUserContext userContext, ITransactionQueriesRepository repository)
        {
            _userContext = userContext;
            _repository = repository;
        }

        public async Task<QueryResult<TransactionInfoViewModel>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            var (totalCount, transactions) =
                await _repository.FindAsync(_userContext.UserId, QueryParams.Create(request),
                    cancellationToken);

            return new QueryResult<TransactionInfoViewModel>(totalCount, transactions);
        }
    }
}
