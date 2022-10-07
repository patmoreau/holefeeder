using Carter;

using FluentValidation;

using Holefeeder.Domain.Features.Accounts;

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
            .WithTags(nameof(Enumerations))
            .WithName(nameof(GetAccountTypes));
    }

    internal record Request : IRequest<IReadOnlyCollection<AccountType>>;

    internal class Validator : AbstractValidator<Request>
    {
    }

    internal class Handler : IRequestHandler<Request, IReadOnlyCollection<AccountType>>
    {
        public Task<IReadOnlyCollection<AccountType>> Handle(Request query, CancellationToken cancellationToken)
        {
            return Task.FromResult(AccountType.List);
        }
    }
}
