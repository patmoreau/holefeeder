using DrifterApps.Seeds.Application.Mediatr;
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
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(request, cancellationToken);
                    return result switch
                    {
                        { IsFailure: true } => result.Error.ToProblem(),
                        _ => Results.CreatedAtRoute(nameof(GetTransaction), new { Id = (Guid)result.Value.FromTransactionId },
                            result.Value)
                    };
                })
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(Transfer))
            .RequireAuthorization(Policies.WriteUser);

    internal record Request(
        DateOnly Date,
        Money Amount,
        string Description,
        AccountId FromAccountId,
        AccountId ToAccountId) : IRequest<Result<(TransactionId FromTransactionId, TransactionId ToTransactionId)>>,
        IUnitOfWorkRequest;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.FromAccountId).NotNull().NotEqual(AccountId.Empty);
            RuleFor(command => command.ToAccountId).NotNull().NotEqual(AccountId.Empty);
            RuleFor(command => command.Date).NotEmpty();
        }
    }

    internal class Handler(IUserContext userContext, BudgetingContext context)
        : IRequestHandler<Request, Result<(TransactionId FromTransactionId, TransactionId ToTransactionId)>>
    {
        public Task<Result<(TransactionId FromTransactionId, TransactionId ToTransactionId)>> Handle(
            Request request, CancellationToken cancellationToken) =>
            ActiveAccountExistsAsync(request.FromAccountId, cancellationToken)
                .OnSuccess(_ => ActiveAccountExistsAsync(request.ToAccountId, cancellationToken))
                .OnSuccess(FindCategoriesAsync(cancellationToken))
                .OnSuccess(CreateTransactionsAsync(request, cancellationToken));

        private async Task<Result<Nothing>> ActiveAccountExistsAsync(AccountId accountId, CancellationToken cancellationToken) =>
            await context.Accounts.AnyAsync(
                x => x.Id == accountId && x.UserId == userContext.Id && !x.Inactive,
                cancellationToken)
                ? Nothing.Value
                : TransactionErrors.AccountNotFound(accountId);

        private Func<Nothing, Task<Result<(Category TransferFrom, Category TransferTo)>>> FindCategoriesAsync(
            CancellationToken cancellationToken) =>
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

        private Func<(Category TransferFrom, Category TransferTo),
                Task<Result<(TransactionId FromTransactionId, TransactionId ToTransactionId)>>>
            CreateTransactionsAsync(Request request, CancellationToken cancellationToken) =>
            async categories =>
            {
                var transactionFrom = await CreateTransactionAsync(request.FromAccountId, categories.TransferFrom, request, cancellationToken);
                if (transactionFrom.IsFailure)
                {
                    return transactionFrom.Error;
                }

                var transactionTo = await CreateTransactionAsync(request.ToAccountId, categories.TransferTo, request, cancellationToken);
                if (transactionTo.IsFailure)
                {
                    return transactionTo.Error;
                }

                return (transactionFrom.Value.Id, transactionTo.Value.Id);
            };

        private async Task<Result<Transaction>> CreateTransactionAsync(AccountId accountId, Category category,
            Request request, CancellationToken cancellationToken)
        {
            var fromAccount = await context.Accounts
                .FirstAsync(x => x.Id == request.FromAccountId && x.UserId == userContext.Id, cancellationToken);
            var toAccount = await context.Accounts
                .FirstAsync(x => x.Id == request.ToAccountId && x.UserId == userContext.Id, cancellationToken);
            var description = request.Description.Trim();
            if (string.IsNullOrEmpty(description))
            {
                description = $"Transfer from '{fromAccount.Name}' to '{toAccount.Name}'";
            }
            var transaction = Transaction.Create(request.Date, request.Amount, description,
                accountId, category.Id, userContext.Id);
            if (transaction.IsFailure)
            {
                return transaction.Error;
            }

            await context.Transactions.AddAsync(transaction.Value, cancellationToken);
            return transaction.Value;
        }
    }
}
