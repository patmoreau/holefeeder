using Holefeeder.Domain.Features.Categories;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Enumerations.Queries;

public class GetCategoryTypes : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/enumerations/get-category-types",
                async (CancellationToken cancellationToken) =>
                {
                    var result = await Handle(cancellationToken);
                    return Results.Ok(result);
                })
            .Produces<IEnumerable<CategoryType>>()
            .WithTags(nameof(Enumerations))
            .WithName(nameof(GetCategoryTypes));

    private static Task<IReadOnlyCollection<CategoryType>> Handle(CancellationToken cancellationToken) =>
        Task.FromResult(CategoryType.List);
}
