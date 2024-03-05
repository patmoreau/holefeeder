using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Application.Mediatr;

using FluentValidation.Results;

using Holefeeder.Application.Context;
using Holefeeder.Application.Features.Transactions.Queries;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Transactions.Commands;

public class Transfer : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("api/v2/transactions/transfer",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result =
                        await mediator.Send(request, cancellationToken);
                    return Results.CreatedAtRoute(nameof(GetTransaction), new { Id = result.FromTransactionId }, result);
                })
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(Transfer))
            .RequireAuthorization();

    internal record Request(DateOnly Date, decimal Amount, string Description, Guid FromAccountId, Guid ToAccountId) : IRequest<(Guid FromTransactionId, Guid ToTransactionId)>, IUnitOfWorkRequest;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.FromAccountId).NotNull().NotEmpty();
            RuleFor(command => command.ToAccountId).NotNull().NotEmpty();
            RuleFor(command => command.Date).NotEmpty();
            RuleFor(command => command.Amount).GreaterThanOrEqualTo(0);
        }
    }

    internal class Handler(IUserContext userContext, BudgetingContext context)
        : IRequestHandler<Request, (Guid FromTransactionId, Guid ToTransactionId)>
    {
        public async Task<(Guid FromTransactionId, Guid ToTransactionId)> Handle(Request request,
            CancellationToken cancellationToken)
        {
            var errors = new List<(string, string)>();

            if (!await ActiveAccountExistsAsync(request.FromAccountId, cancellationToken))
            {
                errors.Add((nameof(request.FromAccountId), $"From account {request.FromAccountId} does not exists"));
            }

            if (!await ActiveAccountExistsAsync(request.ToAccountId, cancellationToken))
            {
                errors.Add((nameof(request.ToAccountId), $"To account {request.ToAccountId} does not exists"));
            }

            var transferTo = await FirstCategoryAsync("Transfer In", cancellationToken);
            var transferFrom = await FirstCategoryAsync("Transfer Out", cancellationToken);

            var transactionFrom = Transaction.Create(request.Date, request.Amount, request.Description,
                request.FromAccountId, transferFrom.Id, userContext.Id);

            await context.Transactions.AddAsync(transactionFrom, cancellationToken);

            var transactionTo = Transaction.Create(request.Date, request.Amount, request.Description,
                request.ToAccountId, transferTo.Id, userContext.Id);

            await context.Transactions.AddAsync(transactionTo, cancellationToken);

            if (errors.Count > 0)
            {
                throw new ValidationException("Transfer error",
                    errors.Select(x => new ValidationFailure(x.Item1, x.Item2)));
            }

            return errors is [_, ..] ? (Guid.Empty, Guid.Empty) : (transactionFrom.Id, transactionTo.Id);
        }

        private async Task<Category> FirstCategoryAsync(string categoryName, CancellationToken cancellationToken) =>
            await context.Categories
                .FirstAsync(x => x.UserId == userContext.Id && x.Name == categoryName, cancellationToken);

        private async Task<bool> ActiveAccountExistsAsync(Guid accountId, CancellationToken cancellationToken) =>
            await context.Accounts.AnyAsync(
                x => x.Id == accountId && x.UserId == userContext.Id && !x.Inactive,
                cancellationToken);
    }
}
