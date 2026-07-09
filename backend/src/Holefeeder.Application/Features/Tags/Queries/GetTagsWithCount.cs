using Holefeeder.Application.Context;
using Holefeeder.Application.UserContext;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Tags.Queries;

public class GetTagsWithCount : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/tags",
                async (IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken) =>
                {
                    var results = await Handle(userContext, context, cancellationToken);
                    return Results.Ok(results);
                })
            .Produces<IEnumerable<TagDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags(nameof(Tags))
            .WithName(nameof(GetTagsWithCount))
            .RequireAuthorization(Policies.ReadUser);

    private static async Task<IEnumerable<TagDto>> Handle(IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        var transactions = await context.Transactions
            .Where(transaction => transaction.UserId == userContext.Id)
            .ToListAsync(cancellationToken: cancellationToken);

        var results = transactions.SelectMany(transaction => transaction.Tags,
                (_, tag) => new { Tag = tag })
            .GroupBy(x => new { x.Tag })
            .Select(group => new TagDto(group.Key.Tag, group.Count()))
            .OrderByDescending(x => x.Count);

        return results;
    }
}
