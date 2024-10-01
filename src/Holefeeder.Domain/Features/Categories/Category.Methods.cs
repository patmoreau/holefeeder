using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Domain.Features.Categories;

public sealed partial record Category
{
    public Result<Category> Modify(CategoryType? type = null, string? name = null, CategoryColor? color = null,
        bool? favorite = null, bool? system = null, Money? budgetAmount = null)
    {
        var newType = type ?? Type;
        var newName = name ?? Name;
        var newColor = color ?? Color;
        var newFavorite = favorite ?? Favorite;
        var newSystem = system ?? System;
        var newBudgetAmount = budgetAmount ?? BudgetAmount;

        var result = Result.Validate(NameValidation(newName));
        if (result.IsFailure)
        {
            return Result<Category>.Failure(result.Error);
        }

        return Result<Category>.Success(new Category(Id, newType, newName, newColor, newBudgetAmount, UserId)
        {
            Favorite = newFavorite,
            System = newSystem,
        });
    }
}
