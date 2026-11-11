using Holefeeder.Service.Accounts.Core.OpenAccount;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Accounts.Api.OpenAccount;

internal static class OpenAccountEndpoint
{
    private const string WriteUserPolicy = "WriteUser";
    private const string GetAccountRoute = "GetAccount";

    public static IEndpointRouteBuilder MapOpenAccount(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/accounts/open-account", HandlerAsync)
            .Produces<OpenAccountResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .WithTags("Accounts")
            .WithName("OpenAccount")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] OpenAccountRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        OpenAccountUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = OpenAccountRequestRaw.ToOpenAccountRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }

        var idResult = await useCase.HandleAsync(requestResult.Value, cancellationToken);
        if (idResult.IsFailure)
        {
            return ApiResult.ToProblem(idResult.Error);
        }
        var id = idResult.Value.Value;

        return Results.CreatedAtRoute(GetAccountRoute, new { id }, new OpenAccountResponse(id));
    }
}
