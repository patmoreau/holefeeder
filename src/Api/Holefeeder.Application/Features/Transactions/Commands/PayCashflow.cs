using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Application.Mediatr;
using Holefeeder.Application.Context;
using Holefeeder.Application.Features.Transactions.Queries;
using Holefeeder.Domain.Features.Transactions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Transactions.Commands;

public class PayCashflow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("api/v2/transactions/pay-cashflow",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    Guid result = await mediator.Send(request, cancellationToken);
                    return Results.CreatedAtRoute(nameof(GetTransaction), new { Id = result }, new { Id = result });
                })
            .Produces<Guid>(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(PayCashflow))
            .RequireAuthorization();

    internal record Request : IRequest<Guid>, IUnitOfWorkRequest
    {
        public DateTime Date { get; init; }

        public decimal Amount { get; init; }

        public Guid CashflowId { get; init; }

        public DateTime CashflowDate { get; init; }
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.Date).NotEmpty();
            RuleFor(command => command.Amount).GreaterThanOrEqualTo(0);
            RuleFor(command => command.CashflowId).NotEmpty();
            RuleFor(command => command.CashflowDate).NotEmpty();
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
            Cashflow? cashflow = await _context.Cashflows.SingleOrDefaultAsync(
                x => x.Id == request.CashflowId && x.UserId == _userContext.UserId, cancellationToken);
            if (cashflow is null)
            {
                throw new ValidationException($"Cashflow '{request.CashflowId}' does not exists");
            }

            Transaction transaction = Transaction.Create(request.Date, request.Amount, cashflow.Description,
                    cashflow.AccountId, cashflow.CategoryId, _userContext.UserId)
                .ApplyCashflow(request.CashflowId, request.CashflowDate);

            transaction = transaction.SetTags(cashflow.Tags.ToArray());

            await _context.Transactions.AddAsync(transaction, cancellationToken);

            return transaction.Id;
        }
    }
}
