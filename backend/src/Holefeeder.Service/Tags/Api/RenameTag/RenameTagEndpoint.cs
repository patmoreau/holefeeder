using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Tags.Core.RenameTag;

namespace Holefeeder.Service.Tags.Api.RenameTag;

internal static class RenameTagEndpoint
{
    private const string WriteUserPolicy = "WriteUser";

    public static IEndpointRouteBuilder MapRenameTag(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/tags/rename", HandlerAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .WithTags("Tags")
            .WithName("RenameTag")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] RenameTagRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        RenameTagUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = RenameTagRequestRaw.ToRenameTagRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }

        var renameResult = await useCase.HandleAsync(requestResult.Value, cancellationToken);
        return renameResult.IsFailure ? ApiResult.ToProblem(renameResult.Error) : Results.NoContent();
    }
}
