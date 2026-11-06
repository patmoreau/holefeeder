using System.Globalization;

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
            .WithName("GetStoreItems")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] SearchStoreItemsRequestRaw request,
        HttpContext context,
        UserContextAccessor userContextAccessor,
        SearchStoreItemsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var pagedResult = await SearchStoreItemsRequestRaw.ToSearchStoreItemsRequest(request, userContextAccessor.Current)
            .OnSuccess(r => useCase.HandleAsync(r, cancellationToken));

        if (pagedResult.IsFailure)
        {
            return pagedResult.Error.ToProblem();
        }

        var paged = pagedResult.Value;
        context.Response.Headers.Append("X-Total-Count", paged.Total.ToString(CultureInfo.InvariantCulture));
        return Results.Ok(paged.Items.Select(item => new StoreItemResponse(item.Id.Value, item.Code.Value, item.Data)));
    }
}
