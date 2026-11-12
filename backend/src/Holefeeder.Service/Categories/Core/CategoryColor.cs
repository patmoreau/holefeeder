using System.Text.RegularExpressions;

using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Categories.Core;

internal sealed partial record CategoryColor
{
    private CategoryColor(string value) => Value = value;

    public string Value { get; }

    public static Result<CategoryColor> Create(string? value) =>
        value is not null && HexColor().IsMatch(value) ? new CategoryColor(value) : CategoryColorErrors.Invalid;

    public static CategoryColor Valid(string value) => new(value);

    [GeneratedRegex("^#[0-9A-Fa-f]{6}$")]
    private static partial Regex HexColor();
}

internal static class CategoryColorErrors
{
    public const string CodeInvalid = "category-color-invalid";

    public static ResultError Invalid => new(CodeInvalid, "Category color must be a hex color as #RRGGBB.");
}
