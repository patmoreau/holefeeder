using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.StoreItems.Core.CreateStoreItem;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.StoreItems.Api.CreateStoreItem;

internal static class CreateStoreItemEndpoint
{
    private const string WriteUserPolicy = "WriteUser";
    private const string GetStoreItemRoute = "GetStoreItem";

    public static IEndpointRouteBuilder MapCreateStoreItem(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/store-items/create-store-item", HandlerAsync)
            .Produces<CreateStoreItemResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .WithTags("StoreItems")
            .WithName("CreateStoreItem")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] CreateStoreItemRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        CreateStoreItemUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = CreateStoreItemRequestRaw.ToCreateStoreItemRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }

        var idResult = await useCase.HandleAsync(requestResult.Value, cancellationToken);
        if (idResult.IsFailure)
        {
            return ApiResult.ToProblem(idResult.Error);
        }
        var id = idResult.Value.Value;

        return Results.CreatedAtRoute(GetStoreItemRoute, new { id }, new CreateStoreItemResponse(id));
    }
}
