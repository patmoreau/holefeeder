using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Holefeeder.Domain.ValueObjects;

[SuppressMessage("Design", "CA1031:Do not catch general exception types")]
public readonly struct CategoryColor : IEquatable<CategoryColor>
{
    public Color Value { get; private init; }

    public override string ToString() => ColorTranslator.ToHtml(Value);

    public static Result<CategoryColor> Create(Color value) =>
        Result<CategoryColor>.Success(new CategoryColor { Value = value });

    public static Result<CategoryColor> Create(string htmlColor)
    {
        try
        {
            var color = ColorTranslator.FromHtml(htmlColor);
            return Result<CategoryColor>.Success(new CategoryColor { Value = color });
        }
        catch (Exception e)
        {
            return Result<CategoryColor>.Failure(CategoryColorErrors.InvalidHtmlColor(e.Message));
        }
    }

    public static implicit operator string(CategoryColor value) => ColorTranslator.ToHtml(value.Value);

    public static CategoryColor Empty => new() { Value = Color.Empty };

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(CategoryColor left, CategoryColor right) => left.Equals(right);

    public static bool operator !=(CategoryColor left, CategoryColor right) => !(left == right);

    public bool Equals(CategoryColor other) => Value.Equals(other.Value);

    public override bool Equals(object? obj) =>
        obj switch
        {
            CategoryColor other => Value.Equals(other.Value),
            Color otherColor => Value.Equals(otherColor),
            string otherString => Value.Equals(ColorTranslator.FromHtml(otherString)),
            _ => false
        };
}

public static class CategoryColorErrors
{
    public const string CodeInvalidHtmlColor = $"{nameof(CategoryColor)}.{nameof(InvalidHtmlColor)}";

    public static ResultError InvalidHtmlColor(string message) =>
        new(CodeInvalidHtmlColor, $"Invalid HTML color: {message}");
}
