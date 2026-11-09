using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.StoreItems.Core.ModifyStoreItem;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.StoreItems.Api.ModifyStoreItem;

internal static class ModifyStoreItemEndpoint
{
    private const string WriteUserPolicy = "WriteUser";

    public static IEndpointRouteBuilder MapModifyStoreItem(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/store-items/modify-store-item", HandlerAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithTags("StoreItems")
            .WithName("ModifyStoreItem")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] ModifyStoreItemRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        ModifyStoreItemUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = ModifyStoreItemRequestRaw.ToModifyStoreItemRequest(rawRequest, userContextAccessor.Current);
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
