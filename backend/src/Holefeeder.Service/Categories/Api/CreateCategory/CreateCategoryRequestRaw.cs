using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Categories.Core;
using Holefeeder.Service.Categories.Core.CreateCategory;
using Holefeeder.Service.Shared.Core;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Categories.Api.CreateCategory;

internal sealed record CreateCategoryRequestRaw([FromBody] CreateCategoryBody Body)
{
    public static Result<CreateCategoryRequest> ToCreateCategoryRequest(CreateCategoryRequestRaw raw, User user)
    {
        var body = raw.Body;
        var name = CategoryName.Create(body.Name);
        var type = CategoryRequestValues.ToType(body.Type);
        var color = CategoryColor.Create(body.Color);
        var budget = Money.Create(body.BudgetAmount);
        var favorite = CategoryRequestValues.ToFavorite(body.Favorite);

        var validation = ResultAggregate.Create();
        AddFailure(validation, name);
        AddFailure(validation, type);
        AddFailure(validation, color);
        AddFailure(validation, budget);
        AddFailure(validation, favorite);

        return validation.OnSuccess<CreateCategoryRequest>(() =>
            new CreateCategoryRequest(user, name.Value, type.Value, color.Value, budget.Value, favorite.Value));
    }

    private static void AddFailure<T>(ResultAggregate validation, Result<T> result)
    {
        if (result.IsFailure)
        {
            validation.AddResult(result.Error);
        }
    }
}
