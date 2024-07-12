using DrifterApps.Seeds.Application.Mediatr;

using Holefeeder.Application.Authorization;
using Holefeeder.Application.Context;
using Holefeeder.Application.Features.Accounts.Exceptions;
using Holefeeder.Application.UserContext;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Accounts.Commands;

public class CloseAccount : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("api/v2/accounts/close-account",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    _ = await mediator.Send(request, cancellationToken);
                    return Results.NoContent();
                })
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(nameof(Accounts))
            .WithName(nameof(CloseAccount))
            .RequireAuthorization(Policies.WriteUser);

    internal class Handler(IUserContext userContext, BudgetingContext context) : IRequestHandler<Request, Unit>
    {
        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var account = await context.Accounts
                // .AsNoTracking()
                .SingleOrDefaultAsync(e => e.Id == request.Id && e.UserId == userContext.Id, cancellationToken);
            if (account is null)
            {
                throw new AccountNotFoundException(request.Id);
            }

            context.Update(account.Close());

            return Unit.Value;
        }
    }

    internal record Request(Guid Id) : IRequest<Unit>, IUnitOfWorkRequest;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator() => RuleFor(command => command.Id).NotNull().NotEmpty();
    }
}
