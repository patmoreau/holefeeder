using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Application.EndpointFilters;
using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Users;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Users.Commands;

public class RegisterUser : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("api/v2/users/register",
                async (IHttpUserContext httpUserContext, BudgetingContext context, CancellationToken cancellationToken) =>
                {
                    var result = await Handle(httpUserContext, context, cancellationToken);
                    return result switch
                    {
                        { IsSuccess: true } => Results.Ok(result.Value),
                        _ => result.Error.ToProblem()
                    };
                })
            .AddEndpointFilter<UnitOfWorkFilter>()
            .Produces<Response>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags(nameof(Users))
            .WithName(nameof(RegisterUser))
            .RequireAuthorization(Policies.WriteUser);

    // Like GetCurrentUser, this resolves the caller from their identity: IUserContext
    // reports UserId.Empty for the unregistered caller this endpoint exists to serve.
    private static async Task<Result<Response>> Handle(IHttpUserContext httpUserContext, BudgetingContext context,
        CancellationToken cancellationToken)
    {
        if (await context.Users.AnyAsync(
                user => user.UserIdentities.Any(identity =>
                    identity.IdentityObjectId == httpUserContext.IdentityObjectId), cancellationToken))
        {
            return UserErrors.AlreadyRegistered;
        }

        var result = User.Register(httpUserContext.IdentityObjectId);
        if (result.IsFailure)
        {
            return result.Error;
        }

        var categories = Category.CreateSystemCategories(result.Value.Id);
        if (categories.IsFailure)
        {
            return categories.Error;
        }

        await context.Users.AddAsync(result.Value, cancellationToken);
        await context.Categories.AddRangeAsync(categories.Value, cancellationToken);

        return new Response((Guid)result.Value.Id);
    }

    public record Response(Guid Id);
}
