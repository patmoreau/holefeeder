using Carter;

using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.SeedWork;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Enumerations.Queries;

public class GetCategoryTypes : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/enumerations/get-category-types",
                async (IMediator mediator) =>
                {
                    var result = await mediator.Send(new Request());
                    return Results.Ok(result);
                })
            .Produces<IEnumerable<CategoryType>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags(nameof(Enumerations))
            .WithName(nameof(GetCategoryTypes))
            .RequireAuthorization();
    }

    public record Request : IRequest<IEnumerable<CategoryType>>;

    public class Handler : IRequestHandler<Request, IEnumerable<CategoryType>>
    {
        public Task<IEnumerable<CategoryType>> Handle(Request query, CancellationToken cancellationToken)
        {
            return Task.FromResult(Enumeration.GetAll<CategoryType>());
        }
    }
}
