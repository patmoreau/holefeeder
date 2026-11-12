using System.Security.Claims;

using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Users.Core.RegisterUser;

namespace Holefeeder.Service.Users.Api.RegisterUser;

internal static class RegisterUserEndpoint
{
    private const string WriteUserPolicy = "WriteUser";

    public static IEndpointRouteBuilder MapRegisterUser(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/users/register", HandlerAsync)
            .Produces<RegisterUserResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Users")
            .WithName("RegisterUser")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal principal,
        RegisterUserUseCase useCase,
        CancellationToken cancellationToken)
    {
        var identityObjectId = IdentityObjectId.Create(principal);
        if (identityObjectId.IsFailure)
        {
            return ApiResult.ToProblem(identityObjectId.Error);
        }

        var idResult = await useCase.HandleAsync(new RegisterUserRequest(identityObjectId.Value), cancellationToken);
        if (idResult.IsFailure)
        {
            return ApiResult.ToProblem(idResult.Error);
        }

        return Results.Ok(new RegisterUserResponse(idResult.Value.Value));
    }
}
