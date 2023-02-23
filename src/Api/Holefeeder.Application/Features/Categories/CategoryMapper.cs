using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Categories;

namespace Holefeeder.Application.Features.Categories;

internal static class CategoryMapper
{
    public static CategoryInfoViewModel MapToCategoryInfoViewModel(Category entity) =>
        new CategoryInfoViewModel(entity.Id, entity.Name, entity.Type, entity.Color);

    public static CategoryViewModel? MapToDtoOrNull(Category? entity) => entity is null ? null : MapToDto(entity);

    public static CategoryViewModel MapToDto(Category entity)
    {
        CategoryViewModel dto = new CategoryViewModel
        {
            Id = entity.Id,
            BudgetAmount = entity.BudgetAmount,
            Color = entity.Color,
            Favorite = entity.Favorite,
            Name = entity.Name
        };

        return dto;
    }

    public static IEnumerable<CategoryViewModel> MapToDto(IEnumerable<Category> entities) => entities.Select(MapToDto);

    public static MyDataCategoryDto MapToMyDataCategoryDto(Category entity)
    {
        MyDataCategoryDto dto = new MyDataCategoryDto
        {
            Id = entity.Id,
            Type = entity.Type,
            BudgetAmount = entity.BudgetAmount,
            Color = entity.Color,
            Favorite = entity.Favorite,
            Name = entity.Name,
            System = entity.System
        };

        return dto;
    }
}
