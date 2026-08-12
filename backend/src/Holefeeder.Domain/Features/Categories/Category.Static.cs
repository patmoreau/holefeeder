using DrifterApps.Seeds.FluentResult;

using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Domain.Features.Categories;

public sealed partial record Category
{
    public static Result<Category> Create(CategoryType type, string name, CategoryColor color, bool favorite,
        bool system, bool inactive, Money budgetAmount, UserId userId)
    {
        var result = ResultAggregate.Create()
            .Ensure(NameValidation(name))
            .Ensure(UserIdValidation(userId));

        return result.OnSuccess(
            () => new Category(CategoryId.New, type, name, color, budgetAmount, userId)
            {
                Favorite = favorite,
                System = system,
                Inactive = inactive
            }.ToResult());
    }

    // Every user needs these two: the transfer feature resolves them by name and
    // fails outright when they are missing.
    public static Result<IReadOnlyCollection<Category>> CreateSystemCategories(UserId userId)
    {
        var transferOut = CreateSystemCategory(CategoryType.Expense, TransferOutName, userId);
        if (transferOut.IsFailure)
        {
            return transferOut.Error;
        }

        var transferIn = CreateSystemCategory(CategoryType.Gain, TransferInName, userId);
        if (transferIn.IsFailure)
        {
            return transferIn.Error;
        }

        IReadOnlyCollection<Category> categories = [transferOut.Value, transferIn.Value];
        return categories.ToResult();
    }

    private static Result<Category> CreateSystemCategory(CategoryType type, string name, UserId userId) =>
        Create(type, name, SystemColor, false, true, false, Money.Zero, userId);

    public static Result<Category> Import(CategoryId id, CategoryType type, string name, CategoryColor color,
        bool favorite, bool system, bool inactive, Money budgetAmount, UserId userId)
    {
        var result = ResultAggregate.Create()
            .Ensure(IdValidation(id))
            .Ensure(NameValidation(name))
            .Ensure(UserIdValidation(userId));

        return result.OnSuccess(
            () => new Category(id, type, name, color, budgetAmount, userId)
            {
                Favorite = favorite,
                System = system,
                Inactive = inactive
            }.ToResult());
    }
}
