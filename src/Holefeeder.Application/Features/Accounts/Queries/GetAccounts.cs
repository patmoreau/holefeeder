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

namespace Holefeeder.Application.Features.Accounts.Queries;

public class GetAccounts : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/accounts",
                async (Request request, IMediator mediator, HttpContext ctx, CancellationToken cancellationToken) =>
                {
                    var (total, accountViewModels) =
                        await mediator.Send(request, cancellationToken);
                    ctx.Response.Headers.Append("X-Total-Count", $"{total}");
                    return Results.Ok(accountViewModels);
                })
            .Produces<IEnumerable<AccountViewModel>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithMetadata(nameof(IRequestQuery))
            .WithTags(nameof(Accounts))
            .WithName(nameof(GetAccounts))
            .RequireAuthorization(Policies.ReadUser);

    internal record Request(int Offset, int Limit, string[] Sort, string[] Filter)
        : IRequest<QueryResult<AccountViewModel>>, IRequestQuery
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter) =>
            context.ToQueryRequest((offset, limit, sort, filter) => new Request(offset, limit, sort, filter));
    }

    internal class Validator : QueryValidatorRoot<Request>;

    internal class Handler(IUserContext userContext, BudgetingContext context)
        : IRequestHandler<Request, QueryResult<AccountViewModel>>
    {
        public async Task<QueryResult<AccountViewModel>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            var total = await context.Accounts.CountAsync(e => e.UserId == userContext.Id, cancellationToken);
            var items = await context.Accounts
                .Include(e => e.Transactions).ThenInclude(x => x.Category)
                .Where(e => e.UserId == userContext.Id)
                .Filter(request.Filter)
                .Sort(request.Sort)
                .Skip(request.Offset)
                .Take(request.Limit)
                .Select(e => AccountMapper.MapToAccountViewModel(e))
                .ToListAsync(cancellationToken);

            return new QueryResult<AccountViewModel>(total, items);
        }
    }
}
