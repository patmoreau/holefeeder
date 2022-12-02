using Holefeeder.Application.Features.Accounts.Exceptions;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.Accounts;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Accounts.Commands;

public class ModifyAccount : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/accounts/modify-account",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    _ = await mediator.Send(request, cancellationToken);
                    return Results.NoContent();
                })
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(nameof(Accounts))
            .WithName(nameof(ModifyAccount))
            .RequireAuthorization();
    }

    internal record Request(Guid Id, string Name, decimal OpenBalance, string Description) : IRequest<Unit>;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.Id).NotNull().NotEmpty();
            RuleFor(command => command.Name).NotNull().NotEmpty().Length(1, 255);
        }
    }

    internal class Handler : IRequestHandler<Request, Unit>
    {
        private readonly IAccountRepository _repository;
        private readonly IUserContext _userContext;

        public Handler(IUserContext userContext, IAccountRepository repository)
        {
            _userContext = userContext;
            _repository = repository;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var exists = await _repository.FindByIdAsync(request.Id, _userContext.UserId, cancellationToken);
            if (exists is null)
            {
                throw new AccountNotFoundException(request.Id);
            }

            try
            {
                var account = exists with
                {
                    Name = request.Name, Description = request.Description, OpenBalance = request.OpenBalance
                };

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
