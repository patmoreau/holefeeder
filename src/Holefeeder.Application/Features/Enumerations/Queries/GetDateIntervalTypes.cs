using Holefeeder.Domain.Enumerations;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Enumerations.Queries;

public class GetDateIntervalTypes : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/enumerations/get-date-interval-types",
                async (IMediator mediator) =>
                {
                    var result = await mediator.Send(new Request());
                    return Results.Ok(result);
                })
            .Produces<IReadOnlyCollection<DateIntervalType>>()
            .WithTags(nameof(Enumerations))
            .WithName(nameof(GetDateIntervalTypes));

    internal record Request : IRequest<IReadOnlyCollection<DateIntervalType>>;

    internal class Validator : AbstractValidator<Request>;

    internal class Handler : IRequestHandler<Request, IReadOnlyCollection<DateIntervalType>>
    {
        public Task<IReadOnlyCollection<DateIntervalType>> Handle(Request query, CancellationToken cancellationToken) =>
            Task.FromResult(DateIntervalType.List);
    }
}
