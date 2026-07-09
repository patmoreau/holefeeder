using Holefeeder.Domain.Enumerations;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Enumerations.Queries;

public class GetDateIntervalTypes : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/enumerations/get-date-interval-types",
                async (CancellationToken cancellationToken) =>
                {
                    var result = await Handle(cancellationToken);
                    return Results.Ok(result);
                })
            .Produces<IReadOnlyCollection<DateIntervalType>>()
            .WithTags(nameof(Enumerations))
            .WithName(nameof(GetDateIntervalTypes));

    private static Task<IReadOnlyCollection<DateIntervalType>> Handle(CancellationToken cancellationToken) =>
        Task.FromResult(DateIntervalType.List);
}
