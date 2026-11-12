using Holefeeder.Service.Categories.Core.DeleteCategory;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Categories.Api.DeleteCategory;

internal static class DeleteCategoryEndpoint
{
    private const string WriteUserPolicy = "WriteUser";

    public static IEndpointRouteBuilder MapDeleteCategory(this IEndpointRouteBuilder app)
    {
        app.MapDelete("api/v2/categories/{id:guid}", HandlerAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithTags("Categories")
            .WithName("DeleteCategory")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] DeleteCategoryRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        DeleteCategoryUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = DeleteCategoryRequestRaw.ToDeleteCategoryRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }

        var deleteResult = await useCase.HandleAsync(requestResult.Value, cancellationToken);
        if (deleteResult.IsFailure)
        {
            return ApiResult.ToProblem(deleteResult.Error);
        }

        return Results.NoContent();
    }
}
