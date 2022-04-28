using Carter;

using FluentValidation;

using Holefeeder.Application.Features.Accounts.Exceptions;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.Accounts;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Holefeeder.Application.Features.Accounts.Commands;

public  class FavoriteAccount : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/accounts/favorite-account",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    _ = await mediator.Send(request, cancellationToken);
                    return Results.NoContent();
                })
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(nameof(Accounts))
            .WithName(nameof(FavoriteAccount))
            .RequireAuthorization();
    }

    public record Request(Guid Id, bool IsFavorite) : IRequest<Unit>;

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.Id).NotNull().NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Request, Unit>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IUserContext _userContext;
        private readonly IAccountRepository _repository;

        public Handler(IUserContext userContext, IAccountRepository repository, ILogger<Handler> logger)
        {
            _userContext = userContext;
            _repository = repository;
            _logger = logger;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var account = await _repository.FindByIdAsync(request.Id, _userContext.UserId, cancellationToken);
            if (account is null)
            {
                throw new AccountNotFoundException(request.Id);
            }

            try
            {
                account = account with {Favorite = request.IsFavorite};

                _logger.LogInformation("----- Set Favorite - Account: {@Account}", account);

                await _repository.SaveAsync(account, cancellationToken);

                await _repository.UnitOfWork.CommitAsync(cancellationToken);

                return Unit.Value;
            }
            catch (AccountDomainException)
            {
                _repository.UnitOfWork.Dispose();
                throw;
            }
        }
    }
}
