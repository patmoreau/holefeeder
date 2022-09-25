using Carter;

using FluentValidation;

using Holefeeder.Domain.Features.Categories;

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
            .WithTags(nameof(Enumerations))
            .WithName(nameof(GetCategoryTypes));
    }

    public record Request : IRequest<IReadOnlyCollection<CategoryType>>;

    public class Validator : AbstractValidator<Request>
    {
    }

    public class Handler : IRequestHandler<Request, IReadOnlyCollection<CategoryType>>
    {
        public Task<IReadOnlyCollection<CategoryType>> Handle(Request query, CancellationToken cancellationToken)
        {
            return Task.FromResult(CategoryType.List);
        }
    }
}