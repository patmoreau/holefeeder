using Holefeeder.Application.Authorization;
using Holefeeder.Application.Context;
using Holefeeder.Application.Features.Accounts.Exceptions;
using Holefeeder.Application.UserContext;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Accounts.Queries;

public class GetAccount : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/accounts/{id:guid}",
                async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var requestResult = await mediator.Send(new Request(id), cancellationToken);
                    return Results.Ok(requestResult);
                })
            .Produces<AccountViewModel>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(nameof(Accounts))
            .WithName(nameof(GetAccount))
            .RequireAuthorization(Policies.ReadUser);

    internal record Request(Guid Id) : IRequest<AccountViewModel>;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator() => RuleFor(x => x.Id).NotEmpty();
    }

    internal class Handler(IUserContext userContext, BudgetingContext context)
        : IRequestHandler<Request, AccountViewModel>
    {
        public async Task<AccountViewModel> Handle(Request query, CancellationToken cancellationToken)
        {
            var account = await context.Accounts
                .Include(e => e.Transactions)
                .ThenInclude(e => e.Category)
                .SingleOrDefaultAsync(x => x.Id == query.Id && x.UserId == userContext.Id,
                    cancellationToken);
            if (account is null)
            {
                throw new AccountNotFoundException(query.Id);
            }

            return new AccountViewModel(
                account.Id,
                account.Type,
                account.Name,
                account.OpenBalance,
                account.OpenDate,
                account.Transactions.Count,
                account.CalculateBalance(),
                account.CalculateLastTransactionDate(),
                account.Description,
                account.Favorite,
                account.Inactive
            );
        }
    }
}
