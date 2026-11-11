using System.Text.Json;

using Holefeeder.Service.Accounts.Core.GetAccountDetail;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Accounts.Api.GetAccountDetail;

internal static class GetAccountDetailEndpoint
{
    private const string ReadUserPolicy = "ReadUser";

    public static IEndpointRouteBuilder MapGetAccountDetail(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/accounts/{id:guid}/detail", HandlerAsync)
            .Produces<AccountDetailResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithTags("Accounts")
            .WithName("GetAccountDetail")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] GetAccountDetailRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        TimeProvider timeProvider,
        GetAccountDetailUseCase useCase,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
        var requestResult = GetAccountDetailRequestRaw.ToGetAccountDetailRequest(rawRequest, userContextAccessor.Current, today);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }
        var request = requestResult.Value;

        var accountResult = await useCase.HandleAsync(request, cancellationToken);
        if (accountResult.IsFailure)
        {
            return ApiResult.ToProblem(accountResult.Error);
        }
        var account = accountResult.Value;

        return Results.Ok(new AccountDetailResponse(
            account.Id.Value,
            account.Name,
            account.Type.Name,
            account.Description,
            account.OpenBalance,
            account.OpenDate,
            account.Favorite,
            account.Inactive,
            JsonNamingPolicy.CamelCase.ConvertName(account.Caption.ToString()),
            account.Balance,
            account.Upcoming,
            account.ProjectedBalance,
            account.Updated));
    }
}
