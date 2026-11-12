using Holefeeder.Service.Categories.Api.ListCategories;
using Holefeeder.Service.Categories.Core;
using Holefeeder.Service.Categories.Core.ListCategories;
using Holefeeder.Service.Categories.Persistence;

namespace Holefeeder.Service.Categories;

internal static class Categories
{
    public static IServiceCollection AddCategoriesServices(this IServiceCollection services) =>
        services
            .AddScoped<ICategories, CategoriesInDatabase>()
            .AddScoped<ListCategoriesUseCase>();

    public static IEndpointRouteBuilder MapCategoriesEndpoints(this IEndpointRouteBuilder app) =>
        app.MapListCategories();
}
