using System.Reflection;

using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Application.Extensions;

using Holefeeder.Application.Authorization;
using Holefeeder.Application.Context;
using Holefeeder.Application.Models;
using Holefeeder.Application.UserContext;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Transactions.Queries;

public class GetTransactions : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/transactions",
                async (Request request, IMediator mediator, HttpContext ctx, CancellationToken cancellationToken) =>
                {
                    var (total, viewModels) =
                        await mediator.Send(request, cancellationToken);
                    ctx.Response.Headers.Append("X-Total-Count", $"{total}");
                    return Results.Ok(viewModels);
                })
            .Produces<IEnumerable<TransactionInfoViewModel>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithMetadata(nameof(IRequestQuery))
            .WithTags(nameof(Transactions))
            .WithName(nameof(GetTransactions))
            .RequireAuthorization(Policies.ReadUser);

    internal record Request(int Offset, int Limit, string[] Sort, string[] Filter)
        : IRequest<QueryResult<TransactionInfoViewModel>>, IRequestQuery
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter) =>
            context.ToQueryRequest((offset, limit, sort, filter) => new Request(offset, limit, sort, filter));
    }

    internal class Validator : QueryValidatorRoot<Request>;

    internal class Handler(IUserContext userContext, BudgetingContext context)
        : IRequestHandler<Request, QueryResult<TransactionInfoViewModel>>
    {
        public async Task<QueryResult<TransactionInfoViewModel>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            var total = await context.Transactions.CountAsync(e => e.UserId == userContext.Id, cancellationToken);
            var items = await context.Transactions
                .Include(e => e.Account)
                .Include(e => e.Category)
                .Where(e => e.UserId == userContext.Id)
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
