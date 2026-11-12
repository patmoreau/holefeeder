using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Features.Categories;

namespace Holefeeder.Application.Features.Categories;

internal static class CategoryMapper
{
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
            System = entity.System,
            Inactive = entity.Inactive
        };

        return dto;
    }
}
