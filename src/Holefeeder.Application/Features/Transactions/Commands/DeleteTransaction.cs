using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Application.Mediatr;

using Holefeeder.Application.Context;
using Holefeeder.Application.Features.Transactions.Exceptions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Transactions.Commands;

public class DeleteTransaction : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapDelete("api/v2/transactions/{id:guid}",
                async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    await mediator.Send(new Request(id), cancellationToken);
                    return Results.NoContent();
                })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(DeleteTransaction))
            .RequireAuthorization();

    internal record Request(Guid Id) : IRequest<Unit>, IUnitOfWorkRequest;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator() => RuleFor(command => command.Id).NotNull().NotEmpty();
    }

    internal class Handler(IUserContext userContext, BudgetingContext context) : IRequestHandler<Request, Unit>
    {
        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var transaction =
                await context.Transactions.SingleOrDefaultAsync(
                    x => x.Id == request.Id && x.UserId == userContext.Id, cancellationToken);
            if (transaction is null)
            {
                throw new TransactionNotFoundException(request.Id);
            }

            context.Remove(transaction);

            return Unit.Value;
        }
    }
}
