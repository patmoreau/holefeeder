using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Infrastructure.Entities;

namespace Holefeeder.Infrastructure.Mapping;

internal static class CategoryMapper
{
    public static CategoryInfoViewModel MapToCategoryInfoViewModel(CategoryEntity entity)
    {
        return new CategoryInfoViewModel(entity.Id, entity.Name, entity.Type, entity.Color);
    }

    public static CategoryViewModel? MapToDtoOrNull(CategoryEntity? entity)
    {
        return entity is null ? null : MapToDto(entity);
    }

    public static CategoryViewModel MapToDto(CategoryEntity entity)
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

    public static IEnumerable<CategoryViewModel> MapToDto(IEnumerable<CategoryEntity> entities)
    {
        return entities.Select(MapToDto);
    }

    public static Category? MapToModelOrNull(CategoryEntity? entity)
    {
        if (entity is null)
        {
            return null;
        }

        var model = new Category(entity.Id, entity.Type, entity.Name, entity.UserId)
        {
            BudgetAmount = entity.BudgetAmount,
            Color = entity.Color,
            Favorite = entity.Favorite,
            System = entity.System
        };

        return model;
    }

    public static CategoryEntity MapToEntity(Category model)
    {
        return new CategoryEntity
        {
            Id = model.Id,
            Name = model.Name,
            Type = model.Type,
            Color = model.Color,
            BudgetAmount = model.BudgetAmount,
            Favorite = model.Favorite,
            System = model.System,
            UserId = model.UserId
        };
    }

    public static MyDataCategoryDto MapToMyDataCategoryDto(CategoryEntity entity)
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
