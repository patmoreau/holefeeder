using DrifterApps.Seeds.FluentResult;

using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Domain.Features.Categories;

public sealed partial record Category
{
    public static Result<Category> Create(CategoryType type, string name, CategoryColor color, bool favorite,
        bool system, Money budgetAmount, UserId userId)
    {
        var result = ResultAggregate.Create()
            .Ensure(NameValidation(name))
            .Ensure(UserIdValidation(userId));

        return result.Switch(
            () => Result<Category>.Success(new Category(CategoryId.New, type, name, color, budgetAmount, userId)
            {
                Favorite = favorite,
                System = system
            }),
            Result<Category>.Failure);
    }

    public static Result<Category> Import(CategoryId id, CategoryType type, string name, CategoryColor color,
        bool favorite, bool system, Money budgetAmount, UserId userId)
    {
        var result = ResultAggregate.Create()
            .Ensure(IdValidation(id))
            .Ensure(NameValidation(name))
            .Ensure(UserIdValidation(userId));

        return result.Switch(
            () => Result<Category>.Success(new Category(id, type, name, color, budgetAmount, userId)
            {
                Favorite = favorite,
                System = system
            }),
            Result<Category>.Failure);
    }
}
