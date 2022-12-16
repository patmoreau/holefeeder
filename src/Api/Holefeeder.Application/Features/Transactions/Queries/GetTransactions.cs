using System.Reflection;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.Models;
using Holefeeder.Application.SeedWork;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

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

    internal record Request(int Offset, int Limit, string[] Sort, string[] Filter)
        : IRequest<QueryResult<TransactionInfoViewModel>>, IRequestQuery
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            return context.ToQueryRequest((offset, limit, sort, filter) => new Request(offset, limit, sort, filter));
        }
    }

    internal class Validator : QueryValidatorRoot<Request>
    {
    }

    internal class Handler : IRequestHandler<Request, QueryResult<TransactionInfoViewModel>>
    {
        private readonly IUserContext _userContext;
        private readonly BudgetingContext _context;

        public Handler(IUserContext userContext, BudgetingContext context)
        {
            _userContext = userContext;
            _context = context;
        }

        public async Task<QueryResult<TransactionInfoViewModel>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            var total = await _context.Transactions.CountAsync(e => e.UserId == _userContext.UserId, cancellationToken);
            var items = await _context.Transactions
                .Include(e => e.Account)
                .Include(e => e.Category)
                .Where(e => e.UserId == _userContext.UserId)
                .Filter(request.Filter)
                .Sort(request.Sort)
                .Skip(request.Offset)
                .Take(request.Limit)
                .Select(e => TransactionMapper.MapToDto(e))
                .ToListAsync(cancellationToken);

            return new QueryResult<TransactionInfoViewModel>(total, items);
        }
    }
}
