using Holefeeder.Service.MyData.Core.GetImportStatus;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.MyData.Api.GetImportStatus;

internal static class GetImportStatusEndpoint
{
    public const string RouteName = "ImportDataStatus";

    private const string ReadUserPolicy = "ReadUser";

    public static IEndpointRouteBuilder MapGetImportStatus(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/my-data/import-status/{id:guid}", Handler)
            .Produces<ImportStatusResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithTags("MyData")
            .WithName(RouteName)
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static IResult Handler(
        [AsParameters] GetImportStatusRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        GetImportStatusUseCase useCase)
    {
        var requestResult = GetImportStatusRequestRaw.ToGetImportStatusRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }

        var statusResult = useCase.Handle(requestResult.Value);
        if (statusResult.IsFailure)
        {
            return ApiResult.ToProblem(statusResult.Error);
        }

        return Results.Ok(ImportStatusResponse.From(statusResult.Value));
    }
}
