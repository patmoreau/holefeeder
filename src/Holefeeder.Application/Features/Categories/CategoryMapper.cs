using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Categories;

namespace Holefeeder.Application.Features.Categories;

internal static class CategoryMapper
{
    public static CategoryInfoViewModel MapToCategoryInfoViewModel(Category entity) =>
        new(entity.Id, entity.Name, entity.Type, entity.Color);

    public static CategoryViewModel MapToDto(Category entity)
    {
        var dto = new CategoryViewModel
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
        var dto = new MyDataCategoryDto
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
