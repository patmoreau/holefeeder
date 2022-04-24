using Carter;

using FluentValidation;

using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Application.Features.Transactions.Queries;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.Transactions;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Holefeeder.Application.Features.Transactions.Commands;

public class Transfer : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/transactions/transfer",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(request, cancellationToken);
                    return Results.CreatedAtRoute(nameof(GetTransaction), new {Id = result}, new {Id = result});
                })
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(Transfer))
            .RequireAuthorization();
    }

    public record Request
        (DateTime Date, decimal Amount, string Description, Guid FromAccountId, Guid ToAccountId) : IRequest<Guid>;

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.FromAccountId).NotNull().NotEmpty();
            RuleFor(command => command.ToAccountId).NotNull().NotEmpty();
            RuleFor(command => command.Date).NotNull();
            RuleFor(command => command.Amount).GreaterThan(0);
        }
    }

    public class Handler : IRequestHandler<Request, Guid>
    {
        private readonly IAccountQueriesRepository _accountQueriesRepository;
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly ILogger _logger;
        private readonly IUserContext _userContext;
        private readonly ITransactionRepository _transactionRepository;

        public Handler(
            IUserContext userContext,
            ITransactionRepository transactionRepository,
            IAccountQueriesRepository accountQueriesRepository,
            ICategoriesRepository categoriesRepository,
            ILogger<Handler> logger)
        {
            _userContext = userContext;
            _transactionRepository = transactionRepository;
            _accountQueriesRepository = accountQueriesRepository;
            _categoriesRepository = categoriesRepository;
            _logger = logger;
        }

        public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
        {
            var errors = new List<string>();

            if (!await _accountQueriesRepository.IsAccountActive(request.FromAccountId, _userContext.UserId,
                    cancellationToken))
            {
                errors.Add($"From account {request.FromAccountId} does not exists");
            }

            if (!await _accountQueriesRepository.IsAccountActive(request.ToAccountId, _userContext.UserId,
                    cancellationToken))
            {
                errors.Add($"To account {request.ToAccountId} does not exists");
            }

            var transferTo =
                await _categoriesRepository.FindByNameAsync(_userContext.UserId, "Transfer In", cancellationToken);
            var transferFrom =
                await _categoriesRepository.FindByNameAsync(_userContext.UserId, "Transfer Out", cancellationToken);

            var transactionFrom = Transaction.Create(request.Date, request.Amount, request.Description,
                transferFrom!.Id,
                request.FromAccountId, _userContext.UserId);

            _logger.LogInformation("----- Transfer Money from Account - Transaction: {@Transaction}", transactionFrom);

            await _transactionRepository.SaveAsync(transactionFrom, cancellationToken);

            var transactionTo = Transaction.Create(request.Date, request.Amount, request.Description, transferTo!.Id,
                request.ToAccountId, _userContext.UserId);

            _logger.LogInformation("----- Transfer Money to Account - Transaction: {@Transaction}", transactionTo);

            await _transactionRepository.SaveAsync(transactionTo, cancellationToken);

            await _transactionRepository.UnitOfWork.CommitAsync(cancellationToken);

            return errors.Any() ? Guid.Empty : transactionFrom.Id;
        }
    }
}
