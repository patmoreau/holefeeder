using Holefeeder.Service.Insights.Core.GetSpending;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Insights.Api.GetSpending;

internal static class GetSpendingEndpoint
{
    private const string ReadUserPolicy = "ReadUser";

    public static IEndpointRouteBuilder MapGetSpending(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/insights/spending", HandlerAsync)
            .Produces<BreakdownResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Insights")
            .WithName("GetSpending")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        UserContextAccessor userContextAccessor,
        TimeProvider timeProvider,
        GetSpendingUseCase useCase,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
        var request = new GetSpendingRequest(userContextAccessor.Current, today);

        var spendingResult = await useCase.HandleAsync(request, cancellationToken);
        return spendingResult.IsFailure
            ? ApiResult.ToProblem(spendingResult.Error)
            : Results.Ok(BreakdownResponse.From(spendingResult.Value));
    }
}
