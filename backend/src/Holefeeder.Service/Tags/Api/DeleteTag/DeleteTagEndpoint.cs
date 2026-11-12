using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Tags.Core.DeleteTag;

namespace Holefeeder.Service.Tags.Api.DeleteTag;

internal static class DeleteTagEndpoint
{
    private const string WriteUserPolicy = "WriteUser";

    public static IEndpointRouteBuilder MapDeleteTag(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/tags/delete", HandlerAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .WithTags("Tags")
            .WithName("DeleteTag")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] DeleteTagRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        DeleteTagUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = DeleteTagRequestRaw.ToDeleteTagRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }

        var deleteResult = await useCase.HandleAsync(requestResult.Value, cancellationToken);
        return deleteResult.IsFailure ? ApiResult.ToProblem(deleteResult.Error) : Results.NoContent();
    }
}
