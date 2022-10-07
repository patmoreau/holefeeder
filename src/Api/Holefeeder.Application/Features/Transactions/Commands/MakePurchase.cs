using Carter;

using FluentValidation;

using Holefeeder.Application.Features.Transactions.Queries;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Transactions;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

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

    internal record Request : IRequest<Guid>
    {
        public DateTime Date { get; init; }

        public decimal Amount { get; init; }

        public string Description { get; init; } = null!;

        public Guid AccountId { get; init; }

        public Guid CategoryId { get; init; }

        public string[] Tags { get; init; } = null!;

        public CashflowRequest? Cashflow { get; init; }

        public record CashflowRequest(DateTime EffectiveDate, DateIntervalType IntervalType, int Frequency,
            int Recurrence);
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator(IUserContext userContext, ITransactionRepository repository)
        {
            RuleFor(command => command.AccountId)
                .NotNull()
                .NotEmpty()
                .MustAsync(async (id, cancellation) =>
                    (await repository.AccountExists(id, userContext.UserId, cancellation)))
                .WithMessage(x => $"Account '{x.AccountId}' does not exists.")
                .WithErrorCode("NotExistsValidator");
            RuleFor(command => command.CategoryId)
                .NotNull()
                .NotEmpty()
                .MustAsync(async (id, cancellation) =>
                    (await repository.CategoryExists(id, userContext.UserId, cancellation)))
                .WithMessage(x => $"Category '{x.CategoryId}' does not exists.")
                .WithErrorCode("NotExistsValidator");
            RuleFor(command => command.Date).NotEmpty();
            RuleFor(command => command.Amount).GreaterThan(0);
        }
    }

    internal class Handler : IRequestHandler<Request, Guid>
    {
        private readonly IUserContext _userContext;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICashflowRepository _cashflowRepository;

        public Handler(IUserContext userContext, ITransactionRepository transactionRepository,
            ICashflowRepository cashflowRepository)
        {
            _userContext = userContext;
            _transactionRepository = transactionRepository;
            _cashflowRepository = cashflowRepository;
        }

        public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
        {
            try
            {
                var cashflowId = await HandleCashflow(request, cancellationToken);

                var transaction = Transaction.Create(request.Date, request.Amount, request.Description,
                        request.AccountId, request.CategoryId, _userContext.UserId);

                if (cashflowId is not null)
                {
                    transaction = transaction.ApplyCashflow(cashflowId.Value, request.Date);
                }

                transaction = transaction.SetTags(request.Tags);

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

        private async Task<Guid?> HandleCashflow(Request request, CancellationToken cancellationToken)
        {
            if (request.Cashflow is null)
            {
                return null;
            }

            var cashflowRequest = request.Cashflow;
            var cashflow = Cashflow.Create(cashflowRequest.EffectiveDate, cashflowRequest.IntervalType,
                cashflowRequest.Frequency, cashflowRequest.Recurrence, request.Amount, request.Description,
                request.CategoryId, request.AccountId, _userContext.UserId);

            cashflow = cashflow.SetTags(request.Tags);

            await _cashflowRepository.SaveAsync(cashflow, cancellationToken);

            return cashflow.Id;
        }
    }
}
