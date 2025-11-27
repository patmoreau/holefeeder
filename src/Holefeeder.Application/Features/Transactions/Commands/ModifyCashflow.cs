using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.Filters;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Domain.ValueObjects;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Transactions.Commands;

public class ModifyCashflow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("api/v2/cashflows/modify",
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
            .WithName(nameof(ModifyCashflow))
            .RequireAuthorization(Policies.WriteUser);

    private static Task<Result<Nothing>> Handle(Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken) =>
        GetExistingCashflow(request, userContext, context, cancellationToken)
            .OnSuccess(ModifyCashflowFunc(request, context));

    private static async Task<Result<Cashflow>> GetExistingCashflow(Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        var cashflow = await context.Cashflows.SingleOrDefaultAsync(
            x => x.Id == request.Id && x.UserId == userContext.Id,
            cancellationToken);
        return cashflow is null
            ? CashflowErrors.NotFound(request.Id)
            : cashflow;
    }

    private static Func<Cashflow, Task<Result<Nothing>>> ModifyCashflowFunc(Request request, BudgetingContext context) =>
        cashflow => Task.FromResult(
            cashflow.Modify(amount: request.Amount, effectiveDate: request.EffectiveDate,
                    description: request.Description)
                .OnSuccess(SetTags(request))
                .OnSuccess(SaveCashflow(context)));

    private static Func<Cashflow, Result<Cashflow>> SetTags(Request request) => cashflow => cashflow.SetTags(request.Tags);

    private static Func<Cashflow, Result<Nothing>> SaveCashflow(BudgetingContext context) =>
        cashflow =>
        {
            context.Update(cashflow);
            return Nothing.Value;
        };

    public record Request
    {
        public required CashflowId Id { get; init; }

        public required Money Amount { get; init; }

        public DateOnly EffectiveDate { get; init; }

        public string Description { get; init; } = null!;

        public string[] Tags { get; init; } = [];
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.Id).NotNull().NotEqual(CashflowId.Empty);
            RuleFor(command => command.EffectiveDate).NotEmpty();
        }
    }
}
