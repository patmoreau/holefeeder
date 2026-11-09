using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.StoreItems.Core.SearchStoreItems;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.StoreItems.Api.SearchStoreItems;

internal static class SearchStoreItemsEndpoint
{
    private const string ReadUserPolicy = "ReadUser";

    public static IEndpointRouteBuilder MapSearchStoreItems(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/store-items", HandlerAsync)
            .Produces<IEnumerable<StoreItemResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .WithTags("StoreItems")
            .WithName("SearchStoreItems")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] SearchStoreItemsRequestRaw rawRequest,
        HttpContext context,
        UserContextAccessor userContextAccessor,
        SearchStoreItemsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = SearchStoreItemsRequestRaw.ToSearchStoreItemsRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }
        var request = requestResult.Value;

        var pagedResult = await useCase.HandleAsync(request, cancellationToken);
        if (pagedResult.IsFailure)
        {
            return ApiResult.ToProblem(pagedResult.Error);
        }
        var paged = pagedResult.Value;

        return ApiResult.ToPagedOk(paged.Total, paged.Items.Select(item => new StoreItemResponse(item.Id.Value, item.Code.Value, item.Data)), context);
    }
}
