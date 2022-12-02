using Holefeeder.Application.Models;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Categories.Queries.GetCategories;

public class GetCategories : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/categories",
                async (IMediator mediator, HttpContext ctx, CancellationToken cancellationToken) =>
                {
                    var (total, viewModels) = await mediator.Send(new Request(), cancellationToken);
                    ctx.Response.Headers.Add("X-Total-Count", $"{total}");
                    return Results.Ok(viewModels);
                })
            .Produces<IEnumerable<CategoryViewModel>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags(nameof(Categories))
            .WithName(nameof(GetCategories))
            .RequireAuthorization();
    }
}
