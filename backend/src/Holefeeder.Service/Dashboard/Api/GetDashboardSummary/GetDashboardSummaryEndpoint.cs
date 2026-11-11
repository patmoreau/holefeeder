using Holefeeder.Service.Dashboard.Core.GetDashboardSummary;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Dashboard.Api.GetDashboardSummary;

internal static class GetDashboardSummaryEndpoint
{
    private const string ReadUserPolicy = "ReadUser";

    public static IEndpointRouteBuilder MapGetDashboardSummary(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/dashboard/summary", HandlerAsync)
            .Produces<DashboardSummaryResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Dashboard")
            .WithName("GetDashboardSummary")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        UserContextAccessor userContextAccessor,
        TimeProvider timeProvider,
        GetDashboardSummaryUseCase useCase,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
        var request = new GetDashboardSummaryRequest(userContextAccessor.Current, today);

        var summaryResult = await useCase.HandleAsync(request, cancellationToken);
        if (summaryResult.IsFailure)
        {
            return ApiResult.ToProblem(summaryResult.Error);
        }
        var summary = summaryResult.Value;

        return Results.Ok(new DashboardSummaryResponse(
            summary.CurrentExpenses,
            summary.ExpenseVariation,
            summary.ExpenseVariationPercentage,
            summary.NetFlow,
            summary.CurrentGains,
            summary.AverageExpenses,
            summary.ProjectedNetFlow));
    }
}
