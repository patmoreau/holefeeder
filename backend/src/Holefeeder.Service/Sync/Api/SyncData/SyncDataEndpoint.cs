using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Sync.Core.SyncData;

namespace Holefeeder.Service.Sync.Api.SyncData;

internal static partial class SyncDataEndpoint
{
    private const string WriteUserPolicy = "WriteUser";

    public static IEndpointRouteBuilder MapSyncData(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/sync/powersync", HandlerAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithTags("Sync")
            .WithName("PowerSync")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] SyncDataRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        SyncDataUseCase useCase,
        ILogger<SyncDataUseCase> logger,
        CancellationToken cancellationToken)
    {
        var user = userContextAccessor.Current;
        var requestResult = SyncDataRequestRaw.ToSyncDataRequest(rawRequest, user);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }

        var request = requestResult.Value;
        LogSyncStarted(logger, request.Operations.Count, user.Id.Value);

        var result = await useCase.HandleAsync(request, cancellationToken);
        if (result.IsFailure)
        {
            LogSyncFailed(logger, user.Id.Value, result.Error.Code);
            return ApiResult.ToProblem(result.Error);
        }

        return Results.NoContent();
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Starting sync of {Operations} operations for user {UserId}")]
    private static partial void LogSyncStarted(ILogger logger, int operations, Guid userId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Sync for user {UserId} failed with {Code}")]
    private static partial void LogSyncFailed(ILogger logger, Guid userId, string code);
}
