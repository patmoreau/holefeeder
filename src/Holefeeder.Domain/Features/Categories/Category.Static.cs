using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Domain.Features.Categories;

public sealed partial record Category
{
    public static Result<Category> Create(CategoryType type, string name, CategoryColor color, bool favorite,
        bool system, Money budgetAmount, UserId userId)
    {
        var result = Result.Validate(NameValidation(name), UserIdValidation(userId));
        if (result.IsFailure)
        {
            return Result<Category>.Failure(result.Error);
        }

        return Result<Category>.Success(new Category(CategoryId.New, type, name, color, budgetAmount, userId)
        {
            Favorite = favorite,
            System = system
        });
    }

    public static Result<Category> Import(CategoryId id, CategoryType type, string name, CategoryColor color,
        bool favorite, bool system, Money budgetAmount, UserId userId)
    {
        var result = Result.Validate(IdValidation(id), NameValidation(name), UserIdValidation(userId));
        if (result.IsFailure)
        {
            return Result<Category>.Failure(result.Error);
        }

        return Result<Category>.Success(new Category(id, type, name, color, budgetAmount, userId)
        {
            Favorite = favorite,
            System = system
        });
    }
}
