using Holefeeder.Service.Dashboard.Core.GetUpcomingFlows;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Dashboard.Api.GetUpcomingFlows;

internal static class GetUpcomingFlowsEndpoint
{
    private const string ReadUserPolicy = "ReadUser";

    public static IEndpointRouteBuilder MapGetUpcomingFlows(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/dashboard/upcoming", HandlerAsync)
            .Produces<IReadOnlyList<DashboardUpcomingFlowResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Dashboard")
            .WithName("GetUpcomingFlows")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        UserContextAccessor userContextAccessor,
        TimeProvider timeProvider,
        GetUpcomingFlowsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
        var request = new GetUpcomingFlowsRequest(userContextAccessor.Current, today);

        var flowsResult = await useCase.HandleAsync(request, cancellationToken);
        if (flowsResult.IsFailure)
        {
            return ApiResult.ToProblem(flowsResult.Error);
        }

        return Results.Ok(flowsResult.Value.Select(flow => new DashboardUpcomingFlowResponse(
            flow.CashflowId.Value,
            flow.Date,
            flow.Title,
            flow.Category,
            flow.CategoryColor,
            flow.Tags,
            flow.Amount)).ToList());
    }
}
