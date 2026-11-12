using Holefeeder.Service.Categories.Api.CreateCategory;
using Holefeeder.Service.Categories.Api.DeleteCategory;
using Holefeeder.Service.Categories.Api.ListCategories;
using Holefeeder.Service.Categories.Api.ModifyCategory;
using Holefeeder.Service.Categories.Core;
using Holefeeder.Service.Categories.Core.CreateCategory;
using Holefeeder.Service.Categories.Core.DeleteCategory;
using Holefeeder.Service.Categories.Core.ListCategories;
using Holefeeder.Service.Categories.Core.ModifyCategory;
using Holefeeder.Service.Categories.Persistence;

namespace Holefeeder.Service.Categories;

internal static class Categories
{
    public static IServiceCollection AddCategoriesServices(this IServiceCollection services) =>
        services
            .AddScoped<ICategories, CategoriesInDatabase>()
            .AddScoped<ListCategoriesUseCase>()
            .AddScoped<CreateCategoryUseCase>()
            .AddScoped<ModifyCategoryUseCase>()
            .AddScoped<DeleteCategoryUseCase>();

    public static IEndpointRouteBuilder MapCategoriesEndpoints(this IEndpointRouteBuilder app) =>
        app.MapListCategories()
            .MapCreateCategory()
            .MapModifyCategory()
            .MapDeleteCategory();
}
