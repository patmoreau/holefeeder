using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Categories.Api;

internal static class CategoryRequestErrors
{
    public const string CodeTypeInvalid = "category-type-invalid";
    public const string CodeFavoriteInvalid = "category-favorite-invalid";

    public static ResultError TypeInvalid => new(CodeTypeInvalid);

    public static ResultError FavoriteInvalid => new(CodeFavoriteInvalid);
}
