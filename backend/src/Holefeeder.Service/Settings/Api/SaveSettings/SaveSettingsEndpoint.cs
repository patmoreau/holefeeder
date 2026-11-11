using Holefeeder.Service.Settings.Core.SaveSettings;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Settings.Api.SaveSettings;

internal static class SaveSettingsEndpoint
{
    private const string WriteUserPolicy = "WriteUser";

    public static IEndpointRouteBuilder MapSaveSettings(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/settings/save-settings", HandlerAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .WithTags("Settings")
            .WithName("SaveSettings")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] SaveSettingsRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        SaveSettingsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = SaveSettingsRequestRaw.ToSaveSettingsRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }

        var saveResult = await useCase.HandleAsync(requestResult.Value, cancellationToken);
        if (saveResult.IsFailure)
        {
            return ApiResult.ToProblem(saveResult.Error);
        }

        return Results.NoContent();
    }
}
