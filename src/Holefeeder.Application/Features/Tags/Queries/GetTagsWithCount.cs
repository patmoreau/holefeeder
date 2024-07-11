// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using Holefeeder.Application.Authorization;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Tags.Queries;

public partial class GetTagsWithCount : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
    app.MapGet("api/v2/tags",
    async (IMediator mediator, CancellationToken cancellationToken) =>
    {
        var results = await mediator.Send(new Request(), cancellationToken);
        return Results.Ok(results);
    })
    .Produces<IEnumerable<TagDto>>()
    .Produces(StatusCodes.Status401Unauthorized)
        .WithTags(nameof(Tags))
        .WithName(nameof(GetTagsWithCount))
        .RequireAuthorization(Policies.ReadUser);
}
