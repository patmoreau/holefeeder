using Carter;

using FluentValidation;

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
        app.MapGet("api/v2/accounts/{id:guid}",
                async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
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

    internal record Request(Guid Id) : IRequest<AccountViewModel>;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, AccountViewModel>
    {
        private readonly IAccountQueriesRepository _repository;
        private readonly IUserContext _userContext;

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
