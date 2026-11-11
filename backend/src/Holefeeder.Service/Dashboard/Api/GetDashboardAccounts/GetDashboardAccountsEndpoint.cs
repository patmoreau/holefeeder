using System.Text.Json;

using Holefeeder.Service.Dashboard.Core.GetDashboardAccounts;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Dashboard.Api.GetDashboardAccounts;

internal static class GetDashboardAccountsEndpoint
{
    private const string ReadUserPolicy = "ReadUser";

    public static IEndpointRouteBuilder MapGetDashboardAccounts(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/dashboard/accounts", HandlerAsync)
            .Produces<IReadOnlyList<DashboardAccountResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Dashboard")
            .WithName("GetDashboardAccounts")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        UserContextAccessor userContextAccessor,
        TimeProvider timeProvider,
        GetDashboardAccountsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
        var request = new GetDashboardAccountsRequest(userContextAccessor.Current, today);

        var accountsResult = await useCase.HandleAsync(request, cancellationToken);
        if (accountsResult.IsFailure)
        {
            return ApiResult.ToProblem(accountsResult.Error);
        }

        return Results.Ok(accountsResult.Value.Select(account => new DashboardAccountResponse(
            account.Id.Value,
            account.Name,
            JsonNamingPolicy.CamelCase.ConvertName(account.Caption.ToString()),
            account.Balance,
            account.ProjectedBalance,
            account.Updated)).ToList());
    }
}
