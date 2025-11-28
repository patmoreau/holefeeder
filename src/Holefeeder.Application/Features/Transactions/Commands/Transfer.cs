using DrifterApps.Seeds.Application.EndpointFilters;
using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.Features.Transactions.Queries;
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

public class Transfer : ICarterModule
{
    public const string CategoryFromName = "Transfer Out";
    public const string CategoryToName = "Transfer In";

    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("api/v2/transactions/transfer",
                async (Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken) =>
                {
                    var result = await Handle(request, userContext, context, cancellationToken);
                    return result switch
                    {
                        { IsFailure: true } => result.Error.ToProblem(),
                        _ => Results.CreatedAtRoute(nameof(GetTransaction), new { Id = (Guid)result.Value.FromTransactionId },
                            result.Value)
                    };
                })
            .AddEndpointFilter<ValidationFilter<Request>>()
            .AddEndpointFilter<UnitOfWorkFilter>()
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(Transfer))
            .RequireAuthorization(Policies.WriteUser);

    private static Task<Result<(TransactionId FromTransactionId, TransactionId ToTransactionId)>> Handle(
        Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken) =>
        ActiveAccountExistsAsync(request.FromAccountId, userContext, context, cancellationToken)
            .OnSuccess(_ => ActiveAccountExistsAsync(request.ToAccountId, userContext, context, cancellationToken))
            .OnSuccess(FindCategoriesAsync(userContext, context, cancellationToken))
            .OnSuccess(CreateTransactionsAsync(request, userContext, context, cancellationToken));

    private static async Task<Result<Nothing>> ActiveAccountExistsAsync(AccountId accountId, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken) =>
        await context.Accounts.AnyAsync(
            x => x.Id == accountId && x.UserId == userContext.Id && !x.Inactive,
            cancellationToken)
            ? Nothing.Value
            : TransactionErrors.AccountNotFound(accountId);

    private static Func<Nothing, Task<Result<(Category TransferFrom, Category TransferTo)>>> FindCategoriesAsync(
        IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken) =>
        async _ =>
        {
            var categoryFrom = await context.Categories
                .FirstOrDefaultAsync(x => x.UserId == userContext.Id && x.Name == CategoryFromName,
                    cancellationToken);
            if (categoryFrom is null)
            {
                return TransactionErrors.CategoryNameNotFound(CategoryFromName);
            }

            var categoryTo = await context.Categories
                .FirstOrDefaultAsync(x => x.UserId == userContext.Id && x.Name == CategoryToName,
                    cancellationToken);
            return categoryTo is null
                ? TransactionErrors.CategoryNameNotFound(CategoryToName)
                : (categoryFrom, categoryTo);
        };

    private static Func<(Category TransferFrom, Category TransferTo),
            Task<Result<(TransactionId FromTransactionId, TransactionId ToTransactionId)>>>
        CreateTransactionsAsync(Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken) =>
        async categories =>
        {
            var fromAccount = await context.Accounts
                .FirstAsync(x => x.Id == request.FromAccountId && x.UserId == userContext.Id, cancellationToken);
            var toAccount = await context.Accounts
                .FirstAsync(x => x.Id == request.ToAccountId && x.UserId == userContext.Id, cancellationToken);
            var requestWithDescription = request with
            {
                Description = string.IsNullOrWhiteSpace(request.Description)
                    ? $"Transfer from '{fromAccount.Name}' to '{toAccount.Name}'"
                    : request.Description
            };

            var transactionFrom = await CreateTransactionAsync(request.FromAccountId, categories.TransferFrom, requestWithDescription, userContext, context, cancellationToken);
            if (transactionFrom.IsFailure)
            {
                return transactionFrom.Error;
            }

            var transactionTo = await CreateTransactionAsync(request.ToAccountId, categories.TransferTo, requestWithDescription, userContext, context, cancellationToken);
            if (transactionTo.IsFailure)
            {
                return transactionTo.Error;
            }

            return (transactionFrom.Value.Id, transactionTo.Value.Id);
        };

    private static async Task<Result<Transaction>> CreateTransactionAsync(AccountId accountId, Category category,
        Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        var transaction = Transaction.Create(request.Date, request.Amount, request.Description,
            accountId, category.Id, userContext.Id);
        if (transaction.IsFailure)
        {
            return transaction.Error;
        }

        await context.Transactions.AddAsync(transaction.Value, cancellationToken);
        return transaction.Value;
    }

    public record Request(
        DateOnly Date,
        Money Amount,
        string Description,
        AccountId FromAccountId,
        AccountId ToAccountId);

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.FromAccountId).NotNull().NotEqual(AccountId.Empty);
            RuleFor(command => command.ToAccountId).NotNull().NotEqual(AccountId.Empty);
            RuleFor(command => command.Date).NotEmpty();
        }
    }
}
