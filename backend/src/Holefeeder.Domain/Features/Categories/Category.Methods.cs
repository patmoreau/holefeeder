using DrifterApps.Seeds.FluentResult;

using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Domain.Features.Categories;

public sealed partial record Category
{
    public Result<Category> Modify(CategoryType? type = null, string? name = null, CategoryColor? color = null,
        bool? favorite = null, bool? system = null, bool? inactive = null, Money? budgetAmount = null)
    {
        var newType = type ?? Type;
        var newName = name ?? Name;
        var newColor = color ?? Color;
        var newFavorite = favorite ?? Favorite;
        var newSystem = system ?? System;
        var newInactive = inactive ?? Inactive;
        var newBudgetAmount = budgetAmount ?? BudgetAmount;

        var result = ResultAggregate.Create()
            .Ensure(NameValidation(newName));

        return result.OnSuccess(
            () => new Category(Id, newType, newName, newColor, newBudgetAmount, UserId)
            {
                Favorite = newFavorite,
                System = newSystem,
                Inactive = newInactive,
            }.ToResult());
    }
}
