using Holefeeder.Application.Context;
using Holefeeder.Application.Features.Transactions.Exceptions;
using Holefeeder.Application.SeedWork;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Transactions.Commands;

public class ModifyCashflow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/cashflows/modify",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    await mediator.Send(request, cancellationToken);
                    return Results.NoContent();
                })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(ModifyCashflow))
            .RequireAuthorization();
    }

    internal record Request : ICommandRequest<Unit>
    {
        public Guid Id { get; init; }

        public decimal Amount { get; init; }

        public string Description { get; init; } = null!;

        public string[] Tags { get; init; } = Array.Empty<string>();
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.Id).NotNull().NotEmpty();
            RuleFor(command => command.Amount).GreaterThan(0);
        }
    }

    internal class Handler : IRequestHandler<Request, Unit>
    {
        private readonly IUserContext _userContext;
        private readonly BudgetingContext _context;

        public Handler(IUserContext userContext, BudgetingContext context)
        {
            _userContext = userContext;
            _context = context;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var exists =
                await _context.Cashflows.SingleOrDefaultAsync(
                    x => x.Id == request.Id && x.UserId == _userContext.UserId,
                    cancellationToken);
            if (exists is null)
            {
                throw new CashflowNotFoundException(request.Id);
            }

            var cashflow = exists with { Amount = request.Amount, Description = request.Description };

            cashflow = cashflow.SetTags(request.Tags);

            _context.Update(cashflow);

            return Unit.Value;
        }
    }
}
