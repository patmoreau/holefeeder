using System.Reflection;

using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Application.EndpointFilters;
using DrifterApps.Seeds.Application.Extensions;
using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.ValueObjects;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Accounts.Queries;

public class GetAccounts : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/accounts",
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
            .Produces<IEnumerable<AccountViewModel>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithMetadata("Query")
            .WithTags(nameof(Accounts))
            .WithName(nameof(GetAccounts))
            .RequireAuthorization(Policies.ReadUser);

    internal record Request(int Offset, int Limit, string[] Sort, string[] Filter) : IRequestQuery
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter) =>
            context.ToQueryRequest((offset, limit, sort, filter) => new Request(offset, limit, sort, filter));
    }

    internal class Validator : QueryValidatorRoot<Request>;

    private static async Task<Result<QueryResult<AccountViewModel>>> Handle(Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        var queryParams = QueryParams.Create(request);
        if (queryParams.IsFailure)
        {
            return queryParams.Error;
        }

        var dateInterval = DateInterval.Create(DateOnly.FromDateTime(DateTime.Now), 0, userContext.Settings.EffectiveDate, userContext.Settings.IntervalType, userContext.Settings.Frequency);

        var total = await context.Accounts.CountAsync(e => e.UserId == userContext.Id, cancellationToken);
        var items = await context.Accounts
            .Include(e => e.Cashflows.Where(c => !c.Inactive)).ThenInclude(x => x.Category)
            .Include(e => e.Transactions).ThenInclude(x => x.Category)
            .Where(e => e.UserId == userContext.Id)
            .Query(queryParams.Value)
            .Select(e => AccountMapper.MapToAccountViewModel(e, dateInterval.End))
            .ToListAsync(cancellationToken);

        return new QueryResult<AccountViewModel>(total, items);
    }
}
