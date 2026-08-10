using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Domain.Features.Users;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Users.Queries;

public class GetCurrentUser : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/users/me",
                async (IHttpUserContext httpUserContext, BudgetingContext context, CancellationToken cancellationToken) =>
                {
                    var result = await Handle(httpUserContext, context, cancellationToken);
                    return result switch
                    {
                        { IsSuccess: true } => Results.Ok(result.Value),
                        _ => result.Error.ToProblem()
                    };
                })
            .Produces<Response>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(nameof(Users))
            .WithName(nameof(GetCurrentUser))
            .RequireAuthorization(Policies.ReadUser);

    // Resolves the caller straight from their identity rather than through IUserContext,
    // which reports UserId.Empty for anyone unregistered — the very case this reports.
    private static async Task<Result<Response>> Handle(IHttpUserContext httpUserContext, BudgetingContext context,
        CancellationToken cancellationToken)
    {
        var userId = await context.Users
            .Where(user => user.UserIdentities.Any(identity =>
                identity.IdentityObjectId == httpUserContext.IdentityObjectId))
            .Select(user => user.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (userId is null)
        {
            return UserErrors.NotFound;
        }

        return new Response((Guid) userId);
    }

    public record Response(Guid Id);
}
