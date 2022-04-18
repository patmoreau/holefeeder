using Carter;

using FluentValidation;

using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.Accounts;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Holefeeder.Application.Features.Accounts.Commands;

public class OpenAccount : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/accounts/open-account",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(request, cancellationToken);
                    return Results.CreatedAtRoute(nameof(GetStoreItem), new {Id = result}, new {Id = result});
                })
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Accounts))
            .WithName(nameof(OpenAccount))
            .RequireAuthorization();
    }

    public record Request(AccountType Type, string Name, DateTime OpenDate, decimal OpenBalance, string Description)
        : IRequest<Guid>;

    public class Validator : AbstractValidator<Request>
    {
        public Validator(IUserContext userContext, IAccountRepository repository)
        {
            RuleFor(command => command.Type).NotNull();
            RuleFor(command => command.Name)
                .NotNull()
                .NotEmpty()
                .Length(1, 255)
                .MustAsync(async (name, cancellation) =>
                    (await repository.FindByNameAsync(name, userContext.UserId, cancellation)) is null)
                .WithMessage(x => $"Name '{x.Name}' already exists.")
                .WithErrorCode("AlreadyExistsValidator");
            RuleFor(command => command.OpenDate).NotNull().NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Request, Guid>
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

        public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
        {
            try
            {
                var account = Account.Create(request.Type, request.Name, request.OpenBalance, request.OpenDate,
                    request.Description, _userContext.UserId);

                _logger.LogInformation("----- Opening Account - Account: {@Account}", account);

                await _repository.SaveAsync(account, cancellationToken);

                await _repository.UnitOfWork.CommitAsync(cancellationToken);

                return account.Id;
            }
            catch (AccountDomainException)
            {
                _repository.UnitOfWork.Dispose();
                throw;
            }
        }
    }
}
