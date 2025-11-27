using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.Filters;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Domain.ValueObjects;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Transactions.Commands;

public class ModifyTransaction : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("api/v2/transactions/modify",
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
            .WithName(nameof(ModifyTransaction))
            .RequireAuthorization(Policies.WriteUser);

    private static async Task<Result<Nothing>> Handle(Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken) =>
        (await CheckIfAccountExits(request, userContext, context, cancellationToken)
            .OnSuccess(_ => CheckIfCategoryExist(request, userContext, context, cancellationToken))
            .OnSuccess(_ => GetExistingTransaction(request, userContext, context, cancellationToken)))
        .OnSuccess(ModifyTransactionFunc(request))
        .OnSuccess(SetTags(request))
        .OnSuccess(SaveTransaction(context));

    private static Func<Transaction, Result<Transaction>> ModifyTransactionFunc(Request request) =>
        transaction => transaction.Modify(
            date: request.Date,
            amount: request.Amount,
            description: request.Description,
            categoryId: request.CategoryId,
            accountId: request.AccountId
        );

    private static async Task<Result<Transaction>> GetExistingTransaction(Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        var transaction = await context.Transactions.SingleOrDefaultAsync(
                x => x.Id == request.Id && x.UserId == userContext.Id, cancellationToken);
        return transaction is null
            ? TransactionErrors.NotFound(request.Id)
            : transaction;
    }

    private static async Task<Result<Nothing>> CheckIfCategoryExist(Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        if (!await context.Categories.AnyAsync(x => x.Id == request.CategoryId && x.UserId == userContext.Id,
                cancellationToken))
        {
            return TransactionErrors.CategoryNotFound(request.CategoryId);
        }

        return Nothing.Value;
    }

    private static async Task<Result<Nothing>> CheckIfAccountExits(Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        if (!await context.Accounts.AnyAsync(x => x.Id == request.AccountId && x.UserId == userContext.Id,
                cancellationToken))
        {
            return TransactionErrors.AccountNotFound(request.AccountId);
        }

        return Nothing.Value;
    }

    private static Func<Transaction, Result<Transaction>> SetTags(Request request) =>
        transaction => transaction.SetTags(request.Tags);

    private static Func<Transaction, Result<Nothing>> SaveTransaction(BudgetingContext context) =>
        transaction =>
        {
            context.Update(transaction);
            return Nothing.Value;
        };

    public record Request
    {
        public required TransactionId Id { get; init; }

        public DateOnly Date { get; init; }

        public Money Amount { get; init; }

        public string Description { get; init; } = null!;

        public required AccountId AccountId { get; init; }

        public required CategoryId CategoryId { get; init; }

        public string[] Tags { get; init; } = null!;
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.Id).NotNull().NotEqual(TransactionId.Empty);
            RuleFor(command => command.AccountId).NotNull().NotEqual(AccountId.Empty);
            RuleFor(command => command.CategoryId).NotNull().NotEqual(CategoryId.Empty);
            RuleFor(command => command.Date).NotNull().NotEmpty();
        }
    }
}
