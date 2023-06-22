using System.Reflection;
using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Application.Extensions;
using Holefeeder.Application.Context;
using Holefeeder.Application.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Transactions.Queries;

public class GetCashflows : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/cashflows",
                async (Request request, IMediator mediator, HttpContext ctx, CancellationToken cancellationToken) =>
                {
                    (int total, IEnumerable<CashflowInfoViewModel> viewModels) =
                        await mediator.Send(request, cancellationToken);
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

    internal record Request(int Offset, int Limit, string[] Sort, string[] Filter)
        : IRequest<QueryResult<CashflowInfoViewModel>>, IRequestQuery
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter) =>
            context.ToQueryRequest((offset, limit, sort, filter) => new Request(offset, limit, sort, filter));
    }

    internal class Validator : QueryValidatorRoot<Request>
    {
    }

    internal class Handler : IRequestHandler<Request, QueryResult<CashflowInfoViewModel>>
    {
        private readonly BudgetingContext _context;
        private readonly IUserContext _userContext;

        public Handler(IUserContext userContext, BudgetingContext context)
        {
            _userContext = userContext;
            _context = context;
        }

        public async Task<QueryResult<CashflowInfoViewModel>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            int total = await _context.Cashflows.CountAsync(e => e.UserId == _userContext.Id, cancellationToken);
            List<CashflowInfoViewModel> items = await _context.Cashflows
                .Include(e => e.Account)
                .Include(e => e.Category)
                .Where(e => e.UserId == _userContext.Id)
                .Filter(request.Filter)
                .Sort(request.Sort)
                .Skip(request.Offset)
                .Take(request.Limit)
                .Select(e => CashflowMapper.MapToDto(e))
                .ToListAsync(cancellationToken);

            return new QueryResult<CashflowInfoViewModel>(total, items);
        }
    }
}
