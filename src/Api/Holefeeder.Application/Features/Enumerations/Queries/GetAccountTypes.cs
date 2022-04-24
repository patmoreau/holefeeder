using Carter;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.SeedWork;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Enumerations.Queries;

public class GetAccountTypes : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/enumerations/get-account-types",
                async (IMediator mediator) =>
                {
                    var result = await mediator.Send(new Request());
                    return Results.Ok(result);
                })
            .Produces<IEnumerable<AccountType>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags(nameof(Enumerations))
            .WithName(nameof(GetAccountTypes))
            .RequireAuthorization();
    }

    public record Request : IRequest<IEnumerable<AccountType>>;

    public class Handler : IRequestHandler<Request, IEnumerable<AccountType>>
    {
        public Task<IEnumerable<AccountType>> Handle(Request query, CancellationToken cancellationToken)
        {
            return Task.FromResult(Enumeration.GetAll<AccountType>());
        }
    }
}
