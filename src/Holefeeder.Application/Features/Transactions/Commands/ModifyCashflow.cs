using DrifterApps.Seeds.Application.Mediatr;

using Holefeeder.Application.Authorization;
using Holefeeder.Application.Context;
using Holefeeder.Application.Features.Transactions.Exceptions;
using Holefeeder.Application.UserContext;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Transactions.Commands;

public class ModifyCashflow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
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
            .RequireAuthorization(Policies.WriteUser);

    internal record Request : IRequest<Unit>, IUnitOfWorkRequest
    {
        public Guid Id { get; init; }

        public decimal Amount { get; init; }

        public DateOnly EffectiveDate { get; init; }

        public string Description { get; init; } = null!;

        public string[] Tags { get; init; } = Array.Empty<string>();
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.Id).NotNull().NotEmpty();
            RuleFor(command => command.Amount).GreaterThanOrEqualTo(0);
            RuleFor(command => command.EffectiveDate).NotEmpty();
        }
    }

    internal class Handler(IUserContext userContext, BudgetingContext context) : IRequestHandler<Request, Unit>
    {
        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var exists =
                await context.Cashflows.SingleOrDefaultAsync(
                    x => x.Id == request.Id && x.UserId == userContext.Id,
                    cancellationToken) ?? throw new CashflowNotFoundException(request.Id);

            var cashflow = exists with { Amount = request.Amount, EffectiveDate = request.EffectiveDate, Description = request.Description };

            cashflow = cashflow.SetTags(request.Tags);

            context.Update(cashflow);

            return Unit.Value;
        }
    }
}
