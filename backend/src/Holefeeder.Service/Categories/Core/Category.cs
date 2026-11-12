using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Categories.Core;

internal sealed record Category(
    Id Id, CategoryName Name, CategoryType Type, CategoryColor Color, Money Budget, bool Favorite, bool System);

internal static class CategoryErrors
{
    public const string CodeNotFound = "category-unknown";
    public const string CodeNameTaken = "category-name-taken";
    public const string CodeSystem = "category-system";

    public static NotFoundError NotFound => new(CodeNotFound, "Category was not found.");

    public static ResultError NameTaken => new(CodeNameTaken, "Category name is already used.");

    public static ResultError System => new(CodeSystem, "System categories cannot be changed.");
}
