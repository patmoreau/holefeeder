using Holefeeder.Application.Context;
using Holefeeder.Application.Features.Transactions.Queries;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.Transactions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Transactions.Commands;

public class Transfer : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/transactions/transfer",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(request, cancellationToken);
                    return Results.CreatedAtRoute(nameof(GetTransaction), new {Id = result.FromTransactionId}, result);
                })
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(Transfer))
            .RequireAuthorization();
    }

    internal record Request
    (DateTime Date, decimal Amount, string Description, Guid FromAccountId,
        Guid ToAccountId) : ICommandRequest<(Guid FromTransactionId, Guid ToTransactionId)>;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.FromAccountId).NotNull().NotEmpty();
            RuleFor(command => command.ToAccountId).NotNull().NotEmpty();
            RuleFor(command => command.Date).NotEmpty();
            RuleFor(command => command.Amount).GreaterThan(0);
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
            var errors = new List<string>();

            if (await _context.Accounts.AnyAsync(
                    x => x.Id == request.FromAccountId && x.UserId == _userContext.UserId && !x.Inactive,
                    cancellationToken))
            {
                errors.Add($"From account {request.FromAccountId} does not exists");
            }

            if (await _context.Accounts.AnyAsync(
                    x => x.Id == request.ToAccountId && x.UserId == _userContext.UserId && !x.Inactive,
                    cancellationToken))
            {
                errors.Add($"To account {request.ToAccountId} does not exists");
            }

            var transferTo =
                await _context.Categories
                    .FirstAsync(x => x.UserId == _userContext.UserId && x.Name == "Transfer In", cancellationToken);
            var transferFrom =
                await _context.Categories
                    .FirstAsync(x => x.UserId == _userContext.UserId && x.Name == "Transfer Out", cancellationToken);

            var transactionFrom = Transaction.Create(request.Date, request.Amount, request.Description,
                request.FromAccountId, transferFrom.Id, _userContext.UserId);

            await _context.Transactions.AddAsync(transactionFrom, cancellationToken);

            var transactionTo = Transaction.Create(request.Date, request.Amount, request.Description,
                request.ToAccountId, transferTo.Id, _userContext.UserId);

            await _context.Transactions.AddAsync(transactionTo, cancellationToken);

            return errors.Any() ? (Guid.Empty, Guid.Empty) : (transactionFrom.Id, transactionTo.Id);
        }
    }
}
