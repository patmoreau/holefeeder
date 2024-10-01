using DrifterApps.Seeds.Application.Mediatr;
using DrifterApps.Seeds.Domain;

using Holefeeder.Application.Authorization;
using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.Features.Transactions;

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
                    var result = await mediator.Send(request, cancellationToken);
                    return result switch
                    {
                        { IsFailure: true } => result.Error.ToProblem(),
                        _ => Results.NoContent()
                    };
                })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(CancelCashflow))
            .RequireAuthorization(Policies.WriteUser);

    internal record Request(CashflowId Id) : IRequest<Result>, IUnitOfWorkRequest;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator() => RuleFor(command => command.Id).NotNull().NotEqual(CashflowId.Empty);
    }

    internal class Handler(IUserContext userContext, BudgetingContext context) : IRequestHandler<Request, Result>
    {
        public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
        {
            var exists =
                await context.Cashflows.SingleOrDefaultAsync(
                    x => x.Id == request.Id && x.UserId == userContext.Id,
                    cancellationToken);
            if (exists is null)
            {
                return Result.Failure(CashflowErrors.NotFound(request.Id));
            }

            var result = exists.Cancel();
            if (result.IsFailure)
            {
                return result;
            }
            context.Update(result.Value);

            return Result.Success();
        }
    }
}
