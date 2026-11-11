using Holefeeder.Service.Cashflows.Core.PayCashflow;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Cashflows.Api.PayCashflow;

internal static class PayCashflowEndpoint
{
    private const string WriteUserPolicy = "WriteUser";

    public static IEndpointRouteBuilder MapPayCashflow(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/cashflows/pay-cashflow", HandlerAsync)
            .Produces<PaymentResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithTags("Cashflows")
            .WithName("CashflowsPayCashflow")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] PayCashflowRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        PayCashflowUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = PayCashflowRequestRaw.ToPayCashflowRequest(rawRequest, userContextAccessor.Current);
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
