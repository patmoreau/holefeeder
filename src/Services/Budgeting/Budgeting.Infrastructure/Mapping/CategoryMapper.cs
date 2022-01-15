using System.Collections.Generic;
using System.Linq;

using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Application.MyData.Models;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.CategoryContext;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Mapping;

public class CategoryMapper
{
    public CategoryInfoViewModel MapToCategoryInfoViewModel(CategoryEntity entity) =>
        new CategoryInfoViewModel(entity.Id, entity.Name, entity.Type, entity.Color);

    public CategoryViewModel? MapToDtoOrNull(CategoryEntity? entity)
    {
        return entity is null ? null : MapToDto(entity);
    }

    public CategoryViewModel MapToDto(CategoryEntity entity)
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

    public IEnumerable<CategoryViewModel> MapToDto(IEnumerable<CategoryEntity> entities) =>
        entities.Select(MapToDto);

    public Category? MapToModelOrNull(CategoryEntity? entity)
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

    public CategoryEntity MapToEntity(Category model)
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

    public MyDataCategoryDto MapToMyDataCategoryDto(CategoryEntity entity)
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
