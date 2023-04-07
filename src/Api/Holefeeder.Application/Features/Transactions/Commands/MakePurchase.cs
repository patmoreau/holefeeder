using Holefeeder.Application.Context;
using Holefeeder.Application.Features.Transactions.Queries;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Transactions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Transactions.Commands;

public class MakePurchase : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("api/v2/transactions/make-purchase",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    Guid result = await mediator.Send(request, cancellationToken);
                    return Results.CreatedAtRoute(nameof(GetTransaction), new { Id = result }, new { Id = result });
                })
            .Produces<Unit>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(MakePurchase))
            .RequireAuthorization();

    internal record Request : ICommandRequest<Guid>
    {
        public DateTime Date { get; init; }

        public decimal Amount { get; init; }

        public string Description { get; init; } = null!;

        public Guid AccountId { get; init; }

        public Guid CategoryId { get; init; }

        public string[] Tags { get; init; } = null!;

        public CashflowRequest? Cashflow { get; init; }

        internal record CashflowRequest(DateTime EffectiveDate, DateIntervalType IntervalType, int Frequency,
            int Recurrence);
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.AccountId).NotNull().NotEmpty();
            RuleFor(command => command.CategoryId).NotNull().NotEmpty();
            RuleFor(command => command.Date).NotEmpty();
            RuleFor(command => command.Amount).GreaterThanOrEqualTo(0);
        }
    }

    internal class Handler : IRequestHandler<Request, Guid>
    {
        private readonly BudgetingContext _context;
        private readonly IUserContext _userContext;

        public Handler(IUserContext userContext, BudgetingContext context)
        {
            _userContext = userContext;
            _context = context;
        }

        public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
        {
            if (!await _context.Accounts.AnyAsync(x => x.Id == request.AccountId && x.UserId == _userContext.UserId,
                    cancellationToken))
            {
                throw new TransactionDomainException($"Account '{request.AccountId}' does not exists.");
            }

            if (!await _context.Categories.AnyAsync(x => x.Id == request.CategoryId && x.UserId == _userContext.UserId,
                    cancellationToken))
            {
                throw new TransactionDomainException($"Category '{request.CategoryId}' does not exists.");
            }

            Guid? cashflowId = await HandleCashflow(request, cancellationToken);

            Transaction transaction = Transaction.Create(request.Date, request.Amount, request.Description,
                request.AccountId, request.CategoryId, _userContext.UserId);

            if (cashflowId is not null)
            {
                transaction = transaction.ApplyCashflow(cashflowId.Value, request.Date);
            }

            transaction = transaction.SetTags(request.Tags);

            await _context.Transactions.AddAsync(transaction, cancellationToken);

            return transaction.Id;
        }

        private async Task<Guid?> HandleCashflow(Request request, CancellationToken cancellationToken)
        {
            if (request.Cashflow is null)
            {
                return null;
            }

            Request.CashflowRequest? cashflowRequest = request.Cashflow;
            Cashflow cashflow = Cashflow.Create(cashflowRequest.EffectiveDate, cashflowRequest.IntervalType,
                cashflowRequest.Frequency, cashflowRequest.Recurrence, request.Amount, request.Description,
                request.CategoryId, request.AccountId, _userContext.UserId);

            cashflow = cashflow.SetTags(request.Tags);

            await _context.Cashflows.AddAsync(cashflow, cancellationToken);

            return cashflow.Id;
        }
    }
}
