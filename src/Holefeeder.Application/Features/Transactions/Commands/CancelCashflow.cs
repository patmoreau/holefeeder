using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Application.Mediatr;

using Holefeeder.Application.Context;
using Holefeeder.Application.Features.Transactions.Exceptions;
using Holefeeder.Application.UserContext;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Transactions.Commands;

public class CancelCashflow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("api/v2/cashflows/cancel",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    await mediator.Send(request, cancellationToken);
                    return Results.NoContent();
                })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(CancelCashflow))
            .RequireAuthorization();

    internal record Request(Guid Id) : IRequest<Unit>, IUnitOfWorkRequest;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator() => RuleFor(command => command.Id).NotNull().NotEmpty();
    }

    internal class Handler(IUserContext userContext, BudgetingContext context) : IRequestHandler<Request, Unit>
    {
        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var exists =
                await context.Cashflows.SingleOrDefaultAsync(
                    x => x.Id == request.Id && x.UserId == userContext.Id,
                    cancellationToken) ?? throw new CashflowNotFoundException(request.Id);
            context.Update(exists.Cancel());

            return Unit.Value;
        }
    }
}
