using Holefeeder.Service.Cashflows.Core.CancelCashflow;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Cashflows.Api.CancelCashflow;

internal static class CancelCashflowEndpoint
{
    private const string WriteUserPolicy = "WriteUser";

    public static IEndpointRouteBuilder MapCancelCashflow(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/cashflows/cancel-cashflow", HandlerAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithTags("Cashflows")
            .WithName("CashflowsCancelCashflow")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] CancelCashflowRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        CancelCashflowUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = CancelCashflowRequestRaw.ToCancelCashflowRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }

        var cancelResult = await useCase.HandleAsync(requestResult.Value, cancellationToken);
        if (cancelResult.IsFailure)
        {
            return ApiResult.ToProblem(cancelResult.Error);
        }

        return Results.NoContent();
    }
}
