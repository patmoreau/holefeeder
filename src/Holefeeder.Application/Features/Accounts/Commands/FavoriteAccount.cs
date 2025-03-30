using DrifterApps.Seeds.Application.Mediatr;
using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.UserContext;
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
                    var result = await mediator.Send(request, cancellationToken);
                    return result switch
                    {
                        { IsFailure: true } => result.Error.ToProblem(),
                        _ => Results.NoContent()
                    };
                })
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(nameof(Accounts))
            .WithName(nameof(FavoriteAccount))
            .RequireAuthorization(Policies.WriteUser);

    internal class Handler(IUserContext userContext, BudgetingContext context) : IRequestHandler<Request, Result<Nothing>>
    {
        public async Task<Result<Nothing>> Handle(Request request, CancellationToken cancellationToken)
        {
            var account = await context.Accounts
                .SingleOrDefaultAsync(x => x.Id == request.Id && x.UserId == userContext.Id, cancellationToken);
            if (account is null)
            {
                return AccountErrors.NotFound(request.Id);
            }

            var result = account.Modify(favorite: request.IsFavorite);
            if (result.IsFailure)
            {
                return result.Error;
            }
            context.Update(result.Value);

            return Nothing.Value;
        }
    }

    public record Request(AccountId Id, bool IsFavorite) : IRequest<Result<Nothing>>, IUnitOfWorkRequest;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator() => RuleFor(command => command.Id).NotNull().NotEqual(AccountId.Empty);
    }
}
