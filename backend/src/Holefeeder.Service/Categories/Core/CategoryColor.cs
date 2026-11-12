namespace Holefeeder.Service.Categories.Core;

internal sealed record CategoryColor
{
    private CategoryColor(string value) => Value = value;

    public string Value { get; }

    public static CategoryColor Valid(string value) => new(value);
}
