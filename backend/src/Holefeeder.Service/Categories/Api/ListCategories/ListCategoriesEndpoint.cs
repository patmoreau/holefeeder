using System.Text.Json;

using Holefeeder.Service.Categories.Core.ListCategories;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Categories.Api.ListCategories;

internal static class ListCategoriesEndpoint
{
    private const string ReadUserPolicy = "ReadUser";

    public static IEndpointRouteBuilder MapListCategories(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/categories/manage", HandlerAsync)
            .Produces<IEnumerable<CategoryResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Categories")
            .WithName("ListCategories")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        UserContextAccessor userContextAccessor,
        ListCategoriesUseCase useCase,
        CancellationToken cancellationToken)
    {
        var categoriesResult = await useCase.HandleAsync(new ListCategoriesRequest(userContextAccessor.Current), cancellationToken);
        if (categoriesResult.IsFailure)
        {
            return ApiResult.ToProblem(categoriesResult.Error);
        }

        return Results.Ok(categoriesResult.Value.Select(category => new CategoryResponse(
            category.Id.Value,
            category.Name.Value,
            JsonNamingPolicy.CamelCase.ConvertName(category.Type.ToString()),
            category.Color.Value,
            category.Budget.Value,
            category.Favorite,
            category.System)));
    }
}
