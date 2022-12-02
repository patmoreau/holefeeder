using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.StoreItems.Commands.ModifyStoreItem;

public class ModifyStoreItem : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/store-items/modify-store-item",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    _ = await mediator.Send(request, cancellationToken);
                    return Results.NoContent();
                })
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(nameof(StoreItems))
            .WithName(nameof(ModifyStoreItem))
            .RequireAuthorization();
    }
}
