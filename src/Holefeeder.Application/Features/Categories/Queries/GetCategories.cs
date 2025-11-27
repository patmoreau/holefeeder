using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.Models;
using Holefeeder.Application.UserContext;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Categories.Queries;

public class GetCategories : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/categories",
                async (IUserContext userContext, BudgetingContext context, HttpContext ctx, CancellationToken cancellationToken) =>
                {
                    var result = await Handle(userContext, context, cancellationToken);
                    switch (result)
                    {
                        case { IsFailure: true }:
                            return result.Error.ToProblem();
                        default:
                            ctx.Response.Headers.Append("X-Total-Count", $"{result.Value.Total}");
                            return Results.Ok(result.Value.Items);
                    }
                })
            .Produces<IEnumerable<CategoryViewModel>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags(nameof(Categories))
            .WithName(nameof(GetCategories))
            .RequireAuthorization(Policies.ReadUser);

    private static async Task<Result<QueryResult<CategoryViewModel>>> Handle(IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        var result = await context.Categories
            .Where(x => x.UserId == userContext.Id)
            .OrderByDescending(x => x.Favorite)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return new QueryResult<CategoryViewModel>(result.Count, CategoryMapper.MapToDto(result));
    }
}
