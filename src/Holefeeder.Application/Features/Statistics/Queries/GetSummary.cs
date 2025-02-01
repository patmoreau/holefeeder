using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Statistics.Queries;

public partial class GetSummary : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/summary/statistics",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var results = await mediator.Send(request, cancellationToken);
                    return Results.Ok(results);
                })
            .Produces<SummaryDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Statistics))
            .WithName(nameof(GetSummary))
            .RequireAuthorization(Policies.ReadUser);
}
