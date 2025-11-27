using System.Reflection;

using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Application.Extensions;
using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.Filters;
using Holefeeder.Application.Models;
using Holefeeder.Application.UserContext;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Transactions.Queries;

public class GetCashflows : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/cashflows",
                async (Request request, IUserContext userContext, BudgetingContext context, HttpContext ctx, CancellationToken cancellationToken) =>
                {
                    var result = await Handle(request, userContext, context, cancellationToken);
                    switch (result)
                    {
                        case { IsFailure: true }:
                            return result.Error.ToProblem();
                        default:
                            ctx.Response.Headers.Append("X-Total-Count", $"{result.Value.Total}");
                            return Results.Ok(result.Value.Items);
                    }
                })
            .AddEndpointFilter<ValidationFilter<Request>>()
            .Produces<IEnumerable<CashflowInfoViewModel>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithMetadata(nameof(IRequestQuery))
            .WithTags(nameof(Transactions))
            .WithName(nameof(GetCashflows))
            .RequireAuthorization(Policies.ReadUser);

    private static async Task<Result<QueryResult<CashflowInfoViewModel>>> Handle(Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        var queryParams = QueryParams.Create(request);
        if (queryParams.IsFailure)
        {
            return queryParams.Error;
        }

        var total = await context.Cashflows.CountAsync(e => e.UserId == userContext.Id, cancellationToken);
        var items = await context.Cashflows
            .Include(e => e.Account)
            .Include(e => e.Category)
            .Where(e => e.UserId == userContext.Id)
            .Query(queryParams.Value)
            .Select(e => CashflowMapper.MapToDto(e))
            .ToListAsync(cancellationToken);

        return new QueryResult<CashflowInfoViewModel>(total, items);
    }

    internal record Request(int Offset, int Limit, string[] Sort, string[] Filter) : IRequestQuery
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter) =>
            context.ToQueryRequest((offset, limit, sort, filter) => new Request(offset, limit, sort, filter));
    }

    internal class Validator : QueryValidatorRoot<Request>;
}
