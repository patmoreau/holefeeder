using Holefeeder.Service.Cashflows.Core.ModifyCashflow;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Cashflows.Api.ModifyCashflow;

internal static class ModifyCashflowEndpoint
{
    private const string WriteUserPolicy = "WriteUser";

    public static IEndpointRouteBuilder MapModifyCashflow(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/cashflows/modify", HandlerAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithTags("Cashflows")
            .WithName("CashflowsModifyCashflow")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] ModifyCashflowRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        ModifyCashflowUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = ModifyCashflowRequestRaw.ToModifyCashflowRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }

        var modifyResult = await useCase.HandleAsync(requestResult.Value, cancellationToken);
        return modifyResult.IsFailure ? ApiResult.ToProblem(modifyResult.Error) : Results.NoContent();
    }
}
