using Carter;

using DrifterApps.Holefeeder.Budgeting.Application.Models;

using Holefeeder.Application.Features.Accounts.Exceptions;
using Holefeeder.Application.SeedWork;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Accounts.Queries;

public class GetAccount : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/accounts/{id}", async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var requestResult = await mediator.Send(new Request(id), cancellationToken);
                return Results.Ok(requestResult);
            })
            .Produces<AccountViewModel>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(nameof(Accounts))
            .WithName(nameof(GetAccount))
            .RequireAuthorization();
    }

    public record Request(Guid Id) : IRequest<AccountViewModel>;

    public class Handler : IRequestHandler<Request, AccountViewModel>
    {
        private readonly IUserContext _userContext;
        private readonly IAccountQueriesRepository _repository;

        public Handler(IUserContext userContext, IAccountQueriesRepository repository)
        {
            _userContext = userContext;
            _repository = repository;
        }

        public async Task<AccountViewModel> Handle(Request query, CancellationToken cancellationToken)
        {
            var result = await _repository.FindByIdAsync(_userContext.UserId, query.Id, cancellationToken);
            if (result is null)
            {
                throw new AccountNotFoundException(query.Id);
            }

            return result;
        }
    }
}
