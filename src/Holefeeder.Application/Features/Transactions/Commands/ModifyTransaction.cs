using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Application.Mediatr;

using Holefeeder.Application.Context;
using Holefeeder.Application.Features.Transactions.Exceptions;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.Features.Transactions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Transactions.Commands;

public class ModifyTransaction : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("api/v2/transactions/modify",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    await mediator.Send(request, cancellationToken);
                    return Results.NoContent();
                })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(ModifyTransaction))
            .RequireAuthorization();

    internal record Request : IRequest<Unit>, IUnitOfWorkRequest
    {
        public Guid Id { get; init; }

        public DateOnly Date { get; init; }

        public decimal Amount { get; init; }

        public string Description { get; init; } = null!;

        public Guid AccountId { get; init; }

        public Guid CategoryId { get; init; }

        public string[] Tags { get; init; } = null!;
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.Id).NotNull().NotEmpty();
            RuleFor(command => command.AccountId).NotNull().NotEmpty();
            RuleFor(command => command.CategoryId).NotNull().NotEmpty();
            RuleFor(command => command.Date).NotNull().NotEmpty();
            RuleFor(command => command.Amount).GreaterThanOrEqualTo(0);
        }
    }

    internal class Handler(IUserContext userContext, BudgetingContext context) : IRequestHandler<Request, Unit>
    {
        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            if (!await context.Accounts.AnyAsync(x => x.Id == request.AccountId && x.UserId == userContext.Id,
                    cancellationToken))
            {
                throw new TransactionDomainException($"Account '{request.AccountId}' does not exists.");
            }

            if (!await context.Categories.AnyAsync(x => x.Id == request.CategoryId && x.UserId == userContext.Id,
                    cancellationToken))
            {
                throw new TransactionDomainException($"Category '{request.CategoryId}' does not exists.");
            }

            var exists =
                await context.Transactions.SingleOrDefaultAsync(
                    x => x.Id == request.Id && x.UserId == userContext.Id, cancellationToken);
            if (exists is null)
            {
                throw new TransactionNotFoundException(request.Id);
            }

            var transaction = exists with
            {
                Date = request.Date,
                Amount = request.Amount,
                Description = request.Description,
                CategoryId = request.CategoryId,
                AccountId = request.AccountId
            };

            transaction = transaction.SetTags(request.Tags);

            context.Update(transaction);

            return Unit.Value;
        }
    }
}
