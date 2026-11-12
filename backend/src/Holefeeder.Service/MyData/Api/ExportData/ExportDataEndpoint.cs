using Holefeeder.Service.MyData.Core.ExportData;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.MyData.Api.ExportData;

internal static class ExportDataEndpoint
{
    private const string ReadUserPolicy = "ReadUser";

    public static IEndpointRouteBuilder MapExportData(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/my-data/export-data", HandlerAsync)
            .Produces<ExportDataResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("MyData")
            .WithName("ExportData")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        UserContextAccessor userContextAccessor,
        ExportDataUseCase useCase,
        CancellationToken cancellationToken)
    {
        var dataResult = await useCase.HandleAsync(new ExportDataRequest(userContextAccessor.Current), cancellationToken);
        if (dataResult.IsFailure)
        {
            return ApiResult.ToProblem(dataResult.Error);
        }

        return Results.Ok(ExportDataResponse.From(dataResult.Value));
    }
}
