using Holefeeder.Service.MyData.Api.GetImportStatus;
using Holefeeder.Service.MyData.Core.ScheduleImport;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

using Microsoft.AspNetCore.Mvc;

namespace Holefeeder.Service.MyData.Api.ImportData;

internal static class ImportDataEndpoint
{
    private const string WriteUserPolicy = "WriteUser";

    public static IEndpointRouteBuilder MapImportData(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/my-data/import-data", Handler)
            .WithMetadata(new DisableRequestSizeLimitAttribute())
            .Produces<ImportDataResponse>(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .WithTags("MyData")
            .WithName("ImportData")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static IResult Handler(
        [AsParameters] ImportDataRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        ScheduleImportUseCase useCase)
    {
        var requestResult = ImportDataRequestRaw.ToImportDataRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }

        var importIdResult = useCase.Handle(requestResult.Value);
        if (importIdResult.IsFailure)
        {
            return ApiResult.ToProblem(importIdResult.Error);
        }
        var id = importIdResult.Value.Value;

        return Results.AcceptedAtRoute(GetImportStatusEndpoint.RouteName, new { id }, new ImportDataResponse(id));
    }
}
