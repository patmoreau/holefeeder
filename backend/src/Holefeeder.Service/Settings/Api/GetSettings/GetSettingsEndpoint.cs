using System.Text.Json;

using Holefeeder.Service.Settings.Core.GetSettings;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Settings.Api.GetSettings;

internal static class GetSettingsEndpoint
{
    private const string ReadUserPolicy = "ReadUser";

    public static IEndpointRouteBuilder MapGetSettings(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/settings", HandlerAsync)
            .Produces<SettingsResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Settings")
            .WithName("GetSettings")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        UserContextAccessor userContextAccessor,
        TimeProvider timeProvider,
        GetSettingsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);

        var settingsResult = await useCase.HandleAsync(new GetSettingsRequest(userContextAccessor.Current, today), cancellationToken);
        if (settingsResult.IsFailure)
        {
            return ApiResult.ToProblem(settingsResult.Error);
        }
        var settings = settingsResult.Value;

        return Results.Ok(new SettingsResponse(
            settings.EffectiveDate,
            JsonNamingPolicy.CamelCase.ConvertName(settings.IntervalType.Name),
            settings.Frequency));
    }
}
