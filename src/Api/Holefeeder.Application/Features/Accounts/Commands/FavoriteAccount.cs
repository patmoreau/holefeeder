using Holefeeder.Application.Context;
using Holefeeder.Application.Features.Accounts.Exceptions;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.Accounts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Accounts.Commands;

public class FavoriteAccount : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("api/v2/accounts/favorite-account",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    _ = await mediator.Send(request, cancellationToken);
                    return Results.NoContent();
                })
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(nameof(Accounts))
            .WithName(nameof(FavoriteAccount))
            .RequireAuthorization();

    internal class Handler : IRequestHandler<Request, Unit>
    {
        private readonly BudgetingContext _context;
        private readonly IUserContext _userContext;

        public Handler(IUserContext userContext, BudgetingContext context)
        {
            _userContext = userContext;
            _context = context;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            Account? account = await _context.Accounts
                .SingleOrDefaultAsync(x => x.Id == request.Id && x.UserId == _userContext.UserId, cancellationToken);
            if (account is null)
            {
                throw new AccountNotFoundException(request.Id);
            }

            _context.Update(account with { Favorite = request.IsFavorite });

            return Unit.Value;
        }
    }

    internal record Request(Guid Id, bool IsFavorite) : ICommandRequest<Unit>;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator() => RuleFor(command => command.Id).NotNull().NotEmpty();
    }
}
