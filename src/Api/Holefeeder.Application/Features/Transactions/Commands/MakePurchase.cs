using Carter;

using FluentValidation;

using Holefeeder.Application.Features.Transactions.Queries;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.Transactions;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Holefeeder.Application.Features.Transactions.Commands;

public class MakePurchase : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/transactions/make-purchase",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(request, cancellationToken);
                    return Results.CreatedAtRoute(nameof(GetTransaction), new {Id = result}, new {Id = result});
                })
            .Produces<Unit>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(MakePurchase))
            .RequireAuthorization();
    }

    public record Request : IRequest<Guid>
    {
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
            RuleFor(command => command.AccountId).NotNull().NotEmpty();
            RuleFor(command => command.CategoryId).NotNull().NotEmpty();
            RuleFor(command => command.Date).NotNull();
            RuleFor(command => command.Amount).GreaterThan(0);
        }
    }

    public class Handler : IRequestHandler<Request, Guid>
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

        public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
        {
            try
            {
                var transaction = Transaction.Create(request.Date, request.Amount, request.Description,
                    request.CategoryId,
                    request.AccountId, _userContext.UserId);

                transaction = transaction.AddTags(request.Tags);

                _logger.LogInformation("----- Making Purchase - Transaction: {@Transaction}", transaction);

                await _transactionRepository.SaveAsync(transaction, cancellationToken);

                await _transactionRepository.UnitOfWork.CommitAsync(cancellationToken);

                return transaction.Id;
            }
            catch (TransactionDomainException)
            {
                _transactionRepository.UnitOfWork.Dispose();
                throw;
            }
        }
    }
}
