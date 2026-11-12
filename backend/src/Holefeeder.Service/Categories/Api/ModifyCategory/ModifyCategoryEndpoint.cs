using Holefeeder.Service.Categories.Core.ModifyCategory;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Categories.Api.ModifyCategory;

internal static class ModifyCategoryEndpoint
{
    private const string WriteUserPolicy = "WriteUser";

    public static IEndpointRouteBuilder MapModifyCategory(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/categories/modify", HandlerAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithTags("Categories")
            .WithName("ModifyCategory")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] ModifyCategoryRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        ModifyCategoryUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = ModifyCategoryRequestRaw.ToModifyCategoryRequest(rawRequest, userContextAccessor.Current);
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
