using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.Filters;
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
                async (Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken) =>
                {
                    var result = await Handle(request, userContext, context, cancellationToken);
                    return result switch
                    {
                        { IsFailure: true } => result.Error.ToProblem(),
                        _ => Results.NoContent()
                    };
                })
            .AddEndpointFilter<ValidationFilter<Request>>()
            .AddEndpointFilter<UnitOfWorkFilter>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(CancelCashflow))
            .RequireAuthorization(Policies.WriteUser);

    private static async Task<Result<Nothing>> Handle(Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        var exists =
            await context.Cashflows.SingleOrDefaultAsync(
                x => x.Id == request.Id && x.UserId == userContext.Id,
                cancellationToken);
        if (exists is null)
        {
            return CashflowErrors.NotFound(request.Id);
        }

        var result = exists.Cancel();
        if (result.IsFailure)
        {
            return result.Error;
        }
        context.Update(result.Value);

        return Nothing.Value;
    }

    public record Request(CashflowId Id);

    internal class Validator : AbstractValidator<Request>
    {
        public Validator() => RuleFor(command => command.Id).NotNull().NotEqual(CashflowId.Empty);
    }
}
