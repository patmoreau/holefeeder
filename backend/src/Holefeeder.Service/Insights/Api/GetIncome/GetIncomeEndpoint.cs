using Holefeeder.Service.Insights.Api;
using Holefeeder.Service.Insights.Core.GetIncome;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Insights.Api.GetIncome;

internal static class GetIncomeEndpoint
{
    private const string ReadUserPolicy = "ReadUser";

    public static IEndpointRouteBuilder MapGetIncome(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/insights/income", HandlerAsync)
            .Produces<BreakdownResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Insights")
            .WithName("GetIncome")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        UserContextAccessor userContextAccessor,
        TimeProvider timeProvider,
        GetIncomeUseCase useCase,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
        var request = new GetIncomeRequest(userContextAccessor.Current, today);

        var incomeResult = await useCase.HandleAsync(request, cancellationToken);
        return incomeResult.IsFailure
            ? ApiResult.ToProblem(incomeResult.Error)
            : Results.Ok(BreakdownResponse.From(incomeResult.Value));
    }
}
