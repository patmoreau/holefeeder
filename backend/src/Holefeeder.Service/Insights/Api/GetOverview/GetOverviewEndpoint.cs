using Holefeeder.Service.Insights.Core.GetOverview;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Insights.Api.GetOverview;

internal static class GetOverviewEndpoint
{
    private const string ReadUserPolicy = "ReadUser";

    public static IEndpointRouteBuilder MapGetOverview(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/insights/overview", HandlerAsync)
            .Produces<OverviewResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Insights")
            .WithName("GetOverview")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] GetOverviewRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        TimeProvider timeProvider,
        GetOverviewUseCase useCase,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
        var requestResult = GetOverviewRequestRaw.ToGetOverviewRequest(rawRequest, userContextAccessor.Current, today);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }

        var overviewResult = await useCase.HandleAsync(requestResult.Value, cancellationToken);
        return overviewResult.IsFailure
            ? ApiResult.ToProblem(overviewResult.Error)
            : Results.Ok(OverviewResponse.From(overviewResult.Value));
    }
}
