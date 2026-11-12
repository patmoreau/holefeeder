using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Categories.Api;

internal static class CategoryRequestValues
{
    public static Result<CategoryType> ToType(string? value) =>
        Enum.TryParse<CategoryType>(value, ignoreCase: true, out var type) && Enum.IsDefined(type) && !int.TryParse(value, out _)
            ? type
            : CategoryRequestErrors.TypeInvalid;

    public static Result<bool> ToFavorite(string? value) =>
        value switch
        {
            null => false,
            _ when bool.TryParse(value, out var favorite) => favorite,
            _ => CategoryRequestErrors.FavoriteInvalid
        };
}
