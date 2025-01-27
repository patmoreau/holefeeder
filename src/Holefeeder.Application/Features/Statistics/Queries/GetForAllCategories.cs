// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Statistics.Queries;

public partial class GetForAllCategories : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/categories/statistics",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var results = await mediator.Send(new Request(), cancellationToken);
                    return Results.Ok(results);
                })
            .Produces<IEnumerable<StatisticsDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Statistics))
            .WithName(nameof(GetForAllCategories))
            .RequireAuthorization(Policies.ReadUser);
}
