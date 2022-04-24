using Carter;

using FluentValidation;

using Holefeeder.Application.Features.Transactions.Exceptions;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.Transactions;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Holefeeder.Application.Features.Transactions.Commands;

public class ModifyTransaction : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/transactions/modify",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(request, cancellationToken);
                    return Results.NoContent();
                })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(ModifyTransaction))
            .RequireAuthorization();
    }

    public record Request : IRequest<Unit>
    {
        public Guid Id { get; init; }

        public DateTime Date { get; init; }

        public decimal Amount { get; init; }

        public string Description { get; init; } = null!;

        public Guid AccountId { get; init; }

        public Guid CategoryId { get; init; }

        public string[] Tags { get; init; } = null!;
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.Id).NotNull().NotEmpty();
            RuleFor(command => command.AccountId).NotNull().NotEmpty();
            RuleFor(command => command.CategoryId).NotNull().NotEmpty();
            RuleFor(command => command.Date).NotNull();
            RuleFor(command => command.Amount).GreaterThan(0);
        }
    }

    public class Handler : IRequestHandler<Request, Unit>
    {
        private readonly ILogger _logger;
        private readonly IUserContext _userContext;
        private readonly ITransactionRepository _transactionRepository;

        public Handler(IUserContext userContext, ITransactionRepository transactionRepository, ILogger<Handler> logger)
        {
            _userContext = userContext;
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            try
            {
                var exists =
                    await _transactionRepository.FindByIdAsync(request.Id, _userContext.UserId, cancellationToken);
                if (exists is null)
                {
                    throw new TransactionNotFoundException(request.Id);
                }

                var transaction = exists with
                {
                    Date = request.Date,
                    Amount = request.Amount,
                    Description = request.Description,
                    CategoryId = request.CategoryId,
                    AccountId = request.AccountId
                };

                transaction = transaction.AddTags(request.Tags);

                _logger.LogInformation("----- Modifying - Transaction: {@Transaction}", transaction);

                await _transactionRepository.SaveAsync(transaction, cancellationToken);

                await _transactionRepository.UnitOfWork.CommitAsync(cancellationToken);

                return Unit.Value;
            }
            catch (TransactionDomainException)
            {
                _transactionRepository.UnitOfWork.Dispose();
                throw;
            }
        }
    }
}
