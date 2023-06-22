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
                    (Guid FromTransactionId, Guid ToTransactionId) result =
                        await mediator.Send(request, cancellationToken);
                    return Results.CreatedAtRoute(nameof(GetTransaction), new { Id = result.FromTransactionId }, result);
                })
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(Transfer))
            .RequireAuthorization();

    internal record Request(DateTime Date, decimal Amount, string Description, Guid FromAccountId, Guid ToAccountId) : IRequest<(Guid FromTransactionId, Guid ToTransactionId)>, IUnitOfWorkRequest;

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

    internal class Handler : IRequestHandler<Request, (Guid FromTransactionId, Guid ToTransactionId)>
    {
        private readonly BudgetingContext _context;
        private readonly IUserContext _userContext;

        public Handler(IUserContext userContext, BudgetingContext context)
        {
            _userContext = userContext;
            _context = context;
        }

        public async Task<(Guid FromTransactionId, Guid ToTransactionId)> Handle(Request request,
            CancellationToken cancellationToken)
        {
            List<(string, string)> errors = new List<(string, string)>();

            if (!await ActiveAccountExistsAsync(request.FromAccountId, cancellationToken))
            {
                errors.Add((nameof(request.FromAccountId), $"From account {request.FromAccountId} does not exists"));
            }

            if (!await ActiveAccountExistsAsync(request.ToAccountId, cancellationToken))
            {
                errors.Add((nameof(request.ToAccountId), $"To account {request.ToAccountId} does not exists"));
            }

            Category transferTo = await FirstCategoryAsync("Transfer In", cancellationToken);
            Category transferFrom = await FirstCategoryAsync("Transfer Out", cancellationToken);

            Transaction transactionFrom = Transaction.Create(request.Date, request.Amount, request.Description,
                request.FromAccountId, transferFrom.Id, _userContext.Id);

            await _context.Transactions.AddAsync(transactionFrom, cancellationToken);

            Transaction transactionTo = Transaction.Create(request.Date, request.Amount, request.Description,
                request.ToAccountId, transferTo.Id, _userContext.Id);

            await _context.Transactions.AddAsync(transactionTo, cancellationToken);

            if (errors.Any())
            {
                throw new ValidationException("Transfer error",
                    errors.Select(x => new ValidationFailure(x.Item1, x.Item2)));
            }

            return errors.Any() ? (Guid.Empty, Guid.Empty) : (transactionFrom.Id, transactionTo.Id);
        }

        private async Task<Category> FirstCategoryAsync(string categoryName, CancellationToken cancellationToken) =>
            await _context.Categories
                .FirstAsync(x => x.UserId == _userContext.Id && x.Name == categoryName, cancellationToken);

        private async Task<bool> ActiveAccountExistsAsync(Guid accountId, CancellationToken cancellationToken) =>
            await _context.Accounts.AnyAsync(
                x => x.Id == accountId && x.UserId == _userContext.Id && !x.Inactive,
                cancellationToken);
    }
}
