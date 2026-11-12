using System.Security.Claims;

using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Users.Core.GetCurrentUser;

namespace Holefeeder.Service.Users.Api.GetCurrentUser;

internal static class GetCurrentUserEndpoint
{
    private const string ReadUserPolicy = "ReadUser";

    public static IEndpointRouteBuilder MapGetCurrentUser(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/users/me", HandlerAsync)
            .Produces<GetCurrentUserResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Users")
            .WithName("GetCurrentUser")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal principal,
        GetCurrentUserUseCase useCase,
        CancellationToken cancellationToken)
    {
        var identityObjectId = IdentityObjectId.Create(principal);
        if (identityObjectId.IsFailure)
        {
            return ApiResult.ToProblem(identityObjectId.Error);
        }

        var idResult = await useCase.HandleAsync(new GetCurrentUserRequest(identityObjectId.Value), cancellationToken);
        if (idResult.IsFailure)
        {
            return ApiResult.ToProblem(idResult.Error);
        }

        return Results.Ok(new GetCurrentUserResponse(idResult.Value.Value));
    }
}
