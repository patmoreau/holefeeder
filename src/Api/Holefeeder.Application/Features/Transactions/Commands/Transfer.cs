using Carter;

using FluentValidation;

using Holefeeder.Application.Context;
using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Application.Features.Transactions.Queries;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.Transactions;

using MediatR;

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
                    return Results.CreatedAtRoute(nameof(GetTransaction), new {Id = result}, new {Id = result});
                })
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(Transfer))
            .RequireAuthorization();
    }

    internal record Request
        (DateTime Date, decimal Amount, string Description, Guid FromAccountId, Guid ToAccountId) : IRequest<Guid>;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.FromAccountId).NotNull().NotEmpty();
            RuleFor(command => command.ToAccountId).NotNull().NotEmpty();
            RuleFor(command => command.Date).NotNull();
            RuleFor(command => command.Amount).GreaterThan(0);
        }
    }

    internal class Handler : IRequestHandler<Request, Guid>
    {
        private readonly IAccountQueriesRepository _accountQueriesRepository;
        private readonly BudgetingContext _context;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserContext _userContext;

        public Handler(
            IUserContext userContext,
            ITransactionRepository transactionRepository,
            IAccountQueriesRepository accountQueriesRepository,
            BudgetingContext context)
        {
            _userContext = userContext;
            _transactionRepository = transactionRepository;
            _accountQueriesRepository = accountQueriesRepository;
            _context = context;
        }

        public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
        {
            var errors = new List<string>();

            if (!await _accountQueriesRepository.IsAccountActive(request.FromAccountId, _userContext.UserId,
                    cancellationToken))
            {
                errors.Add($"From account {request.FromAccountId} does not exists");
            }

            if (!await _accountQueriesRepository.IsAccountActive(request.ToAccountId, _userContext.UserId,
                    cancellationToken))
            {
                errors.Add($"To account {request.ToAccountId} does not exists");
            }

            var transferTo =
                await _context.Categories.AsQueryable()
                    .FirstAsync(x => x.UserId == _userContext.UserId && x.Name == "Transfer In", cancellationToken);
            var transferFrom =
                await _context.Categories.AsQueryable()
                    .FirstAsync(x => x.UserId == _userContext.UserId && x.Name == "Transfer Out", cancellationToken);

            var transactionFrom = Transaction.Create(request.Date, request.Amount, request.Description,
                request.FromAccountId, transferFrom.Id, _userContext.UserId);

            await _transactionRepository.SaveAsync(transactionFrom, cancellationToken);

            var transactionTo = Transaction.Create(request.Date, request.Amount, request.Description,
                request.ToAccountId, transferTo.Id, _userContext.UserId);

            await _transactionRepository.SaveAsync(transactionTo, cancellationToken);

            await _transactionRepository.UnitOfWork.CommitAsync(cancellationToken);

            return errors.Any() ? Guid.Empty : transactionFrom.Id;
        }
    }
}
