using Holefeeder.Service.Cashflows.Core.ClearCashflow;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Cashflows.Api.ClearCashflow;

internal static class ClearCashflowEndpoint
{
    private const string WriteUserPolicy = "WriteUser";

    public static IEndpointRouteBuilder MapClearCashflow(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/cashflows/clear-cashflow", HandlerAsync)
            .Produces<PaymentResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithTags("Cashflows")
            .WithName("CashflowsClearCashflow")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] ClearCashflowRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        ClearCashflowUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = ClearCashflowRequestRaw.ToClearCashflowRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }

        var idResult = await useCase.HandleAsync(requestResult.Value, cancellationToken);
        if (idResult.IsFailure)
        {
            return ApiResult.ToProblem(idResult.Error);
        }

        return Results.Created((string?)null, new PaymentResponse(idResult.Value.Value));
    }
}
