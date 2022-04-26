using Carter;

using FluentValidation;

using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.SeedWork;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Enumerations.Queries;

public class GetDateIntervalTypes : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/enumerations/get-date-interval-types",
                async (IMediator mediator) =>
                {
                    var result = await mediator.Send(new Request());
                    return Results.Ok(result);
                })
            .Produces<IEnumerable<DateIntervalType>>()
            .WithTags(nameof(Enumerations))
            .WithName(nameof(GetDateIntervalTypes));
    }

    public record Request : IRequest<IEnumerable<DateIntervalType>>;

    public class Validator : AbstractValidator<Request>
    {
    }

    public class Handler : IRequestHandler<Request, IEnumerable<DateIntervalType>>
    {
        public Task<IEnumerable<DateIntervalType>> Handle(Request query, CancellationToken cancellationToken)
        {
            return Task.FromResult(Enumeration.GetAll<DateIntervalType>());
        }
    }
}
