using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Categories.Core;

internal sealed record CategoryName
{
    private const int MaxLength = 255;

    private CategoryName(string value) => Value = value;

    public string Value { get; }

    public static Result<CategoryName> Create(string? value) =>
        value?.Trim() switch
        {
            null or { Length: 0 or > MaxLength } => CategoryNameErrors.Invalid,
            var name => new CategoryName(name)
        };

    public static CategoryName Valid(string value) => new(value);
}

internal static class CategoryNameErrors
{
    public const string CodeInvalid = "category-name-invalid";

    public static ResultError Invalid => new(CodeInvalid, "Category name must be between 1 and 255 characters.");
}
