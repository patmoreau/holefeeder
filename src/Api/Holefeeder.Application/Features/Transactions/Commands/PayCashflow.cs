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

public class PayCashflow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/transactions/pay-cashflow",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(request, cancellationToken);
                    return Results.CreatedAtRoute(nameof(GetTransaction), new {Id = result}, new {Id = result});
                })
            .Produces<Guid>(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(PayCashflow))
            .RequireAuthorization();
    }

    public record Request : IRequest<Guid>
    {
        public DateTime Date { get; init; }

        public decimal Amount { get; init; }

        public Guid CashflowId { get; init; }

        public DateTime CashflowDate { get; init; }
    }

    public class Handler : IRequestHandler<Request, Guid>
    {
        private readonly ICashflowRepository _cashflowRepository;
        private readonly ILogger _logger;
        private readonly IUserContext _userContext;
        private readonly ITransactionRepository _transactionRepository;

        public Handler(IUserContext userContext, ITransactionRepository transactionRepository,
            ICashflowRepository cashflowRepository, ILogger<Handler> logger)
        {
            _userContext = userContext;
            _transactionRepository = transactionRepository;
            _cashflowRepository = cashflowRepository;
            _logger = logger;
        }

        public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
        {
            var cashflow =
                await _cashflowRepository.FindByIdAsync(request.CashflowId, _userContext.UserId, cancellationToken);
            if (cashflow is null)
            {
                throw new ValidationException($"Cashflow '{request.CashflowId}' does not exists");
            }

            var transaction = Transaction.Create(request.Date, request.Amount, cashflow.Description,
                cashflow.CategoryId, cashflow.AccountId, request.CashflowId, request.CashflowDate,
                _userContext.UserId);

            transaction = transaction.AddTags(cashflow.Tags.ToArray());

            _logger.LogInformation("----- Pay Cashflow - Transaction: {@Transaction}", transaction);

            await _transactionRepository.SaveAsync(transaction, cancellationToken);

            await _transactionRepository.UnitOfWork.CommitAsync(cancellationToken);

            return transaction.Id;
        }
    }
}
