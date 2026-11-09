using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.StoreItems.Core.GetStoreItem;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.StoreItems.Api.GetStoreItem;

internal static class GetStoreItemEndpoint
{
    private const string ReadUserPolicy = "ReadUser";

    public static IEndpointRouteBuilder MapGetStoreItem(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/store-items/{id:guid}", HandlerAsync)
            .Produces<StoreItemResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithTags("StoreItems")
            .WithName("GetStoreItem")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] GetStoreItemRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        GetStoreItemUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = GetStoreItemRequestRaw.ToGetStoreItemRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }
        var request = requestResult.Value;

        var itemResult = await useCase.HandleAsync(request, cancellationToken);
        if (itemResult.IsFailure)
        {
            return ApiResult.ToProblem(itemResult.Error);
        }
        var item = itemResult.Value;

        return Results.Ok(new StoreItemResponse(item.Id.Value, item.Code.Value, item.Data));
    }
}
