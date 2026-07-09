using DrifterApps.Seeds.Application.EndpointFilters;
using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.Features.Transactions.Queries;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Domain.ValueObjects;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Transactions.Commands;

public class MakePurchase : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("api/v2/transactions/make-purchase",
                async (Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken) =>
                {
                    var result = await Handle(request, userContext, context, cancellationToken);
                    return result switch
                    {
                        { IsFailure: true } => result.Error.ToProblem(),
                        _ => Results.CreatedAtRoute(nameof(GetTransaction), new { Id = (Guid)result.Value },
                            new { Id = (Guid)result.Value })
                    };
                })
            .AddEndpointFilter<ValidationFilter<Request>>()
            .AddEndpointFilter<UnitOfWorkFilter>()
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(MakePurchase))
            .RequireAuthorization(Policies.WriteUser);

    private static async Task<Result<TransactionId>> Handle(Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        if (!await context.Accounts.AnyAsync(x => x.Id == request.AccountId && x.UserId == userContext.Id,
                cancellationToken))
        {
            TransactionErrors.AccountNotFound(request.AccountId);
        }

        if (!await context.Categories.AnyAsync(x => x.Id == request.CategoryId && x.UserId == userContext.Id,
                cancellationToken))
        {
            return TransactionErrors.CategoryNotFound(request.CategoryId);
        }

        var cashflowId = await HandleCashflow(request, userContext, context, cancellationToken);
        if (cashflowId.IsFailure)
        {
            return cashflowId.Error;
        }

        var transaction = Transaction.Create(request.Date, request.Amount, request.Description,
            request.AccountId, request.CategoryId, userContext.Id);
        if (transaction.IsFailure)
        {
            return transaction.Error;
        }

        if (cashflowId.Value is not null && cashflowId.Value != CashflowId.Empty)
        {
            transaction = transaction.Value.ApplyCashflow(cashflowId.Value, request.Date);
            if (transaction.IsFailure)
            {
                return transaction.Error;
            }
        }

        transaction = transaction.Value.SetTags(request.Tags);
        if (transaction.IsFailure)
        {
            return transaction.Error;
        }

        await context.Transactions.AddAsync(transaction.Value, cancellationToken);

        return transaction.Value.Id;
    }

    private static async Task<Result<CashflowId?>> HandleCashflow(Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        if (request.Cashflow is null)
        {
            return CashflowId.Empty;
        }

        var cashflowRequest = request.Cashflow;
        var result = Cashflow.Create(cashflowRequest.EffectiveDate, cashflowRequest.IntervalType,
            cashflowRequest.Frequency, cashflowRequest.Recurrence, request.Amount, request.Description,
            request.CategoryId, request.AccountId, userContext.Id);

        if (result.IsFailure)
        {
            return result.Error;
        }

        var cashflow = result.Value.SetTags(request.Tags);
        if (cashflow.IsFailure)
        {
            return cashflow.Error;
        }

        await context.Cashflows.AddAsync(cashflow.Value, cancellationToken);

        return cashflow.Value.Id;
    }

    public record Request
    {
        public DateOnly Date { get; init; }

        public Money Amount { get; init; }

        public string Description { get; init; } = null!;

        public AccountId AccountId { get; init; } = AccountId.Empty;

        public CategoryId CategoryId { get; init; } = CategoryId.Empty;

        public string[] Tags { get; init; } = null!;

        public CashflowRequest? Cashflow { get; init; }

        public record CashflowRequest(
            DateOnly EffectiveDate,
            DateIntervalType IntervalType,
            int Frequency,
            int Recurrence);
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.AccountId).NotNull().NotEqual(AccountId.Empty);
            RuleFor(command => command.CategoryId).NotNull().NotEqual(CategoryId.Empty);
            RuleFor(command => command.Date).NotEmpty();
        }
    }
}
